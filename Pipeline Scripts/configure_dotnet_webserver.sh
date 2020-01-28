#!/bin/bash

if [ "$1" == "" ]; then
	echo "Usage: $0 <Project.dll>"
	exit 1
else
	strl=${#1}
	let "strl = strl - 4"
	ext=${1:$strl:4}
	if [ "$ext" != ".dll" ]; then
		echo "Name of <Project.dll> must end in .dll"
		exit 1
	else
		dllname=$(echo "$1" | sed "s/\s/\\\\\\\\ /g")
	fi
fi

# https://unix.stackexchange.com/questions/28791/prompt-for-sudo-password-and-programmatically-elevate-privilege-in-bash-script
if [ $UID -ne 0 ]; then
	echo This script was not executed as root.
	echo Rerunning the script with sudo...
	exec sudo bash "$0" "$@";
else

	echo Making kestrel_webapp folder...
	mkdir -p /var/www/kestrel_webapp
	chown apache:webservice -R /var/www/kestrel_webapp/
	chmod 775 -R /var/www/kestrel_webapp/
	chmod g+s /var/www/kestrel_webapp

	echo Creating webservice group...
	sudo groupadd webservice
	echo Done creating webservice group.

	echo Upgrading packages...
	dnf upgrade -q -y
	echo Done upgrading packages.

	echo Installing httpd...
	dnf install -q -y httpd mod_ssl
	echo Giving apache user ownership of /usr/share/httpd...
	chown -R apache:apache /usr/share/httpd/
	echo Adding apache user to webservice system group...
	sudo usermod -aG webservice apache
	echo Configuring httpd to start on boot...
	systemctl --quiet enable httpd 1>/dev/null
	echo Done installing httpd.
	
	echo Installing ASP.NET Core runtime...
	rpm -U https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm 1>/dev/null
	dnf install -q -y aspnetcore-runtime-3.1
	echo Done installing ASP.NET Core runtime.
	
	echo Configuring SELinux to allow Apache to act as reverse proxy...
	sudo /usr/sbin/setsebool -P httpd_can_network_connect 1 1>/dev/null
	echo Done configuring SELinux for Apache reverse proxy.
	
	echo Creating ASP.NET web app reverse proxy config file for Apache...
	printf "<VirtualHost *:*>
    RequestHeader set \"X-Forwarded-Proto\" expr=%%{REQUEST_SCHEME}
</VirtualHost>

<VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://127.0.0.1:5000/
    ProxyPassReverse / http://127.0.0.1:5000/
    ServerName www.example.com
    ServerAlias *.example.com
    ErrorLog /var/log/httpd/kestrel_webapp-error.log
    CustomLog /var/log/httpd/kestrel_webapp-access.log common
</VirtualHost>" | tee /etc/httpd/conf.d/kestrel_webapp.conf 1>/dev/null
	echo Done creating reverse proxy config file.
	
	echo Creating service for Kestrel web server...
	useradd -m --shell /bin/false -G webservice kestrel
	printf "[Unit]
Description=ASP.NET MVC web server

[Service]
WorkingDirectory=/var/www/kestrel_webapp
ExecStart=/bin/bash /usr/local/bin/kestrel_webapp/start.sh
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
# Prevent constant restarts. Not allowed to auto-restart more than 3 times in a minute
StartLimitBurst=3
StartLimitIntervalSec=60
KillSignal=SIGINT
SyslogIdentifier=kestrel-webapp
User=kestrel
Environment=ASPNETCORE_ENVIRONMENT=Production
TimeoutStopSec=90

[Install]
WantedBy=multi-user.target
" | tee /etc/systemd/system/kestrel-webapp.service 1>/dev/null
	echo Creating kestrel-webapp start script...
	mkdir -p /usr/local/bin/kestrel_webapp/
	printf "/usr/bin/dotnet /var/www/kestrel_webapp/$dllname" | tee /usr/local/bin/kestrel_webapp/start.sh 1>/dev/null
	chown kestrel:kestrel /usr/local/bin/kestrel_webapp/*
	chmod u+x /usr/local/bin/kestrel_webapp/start.sh
	echo Setting service to start on boot...
	systemctl --quiet enable kestrel-webapp --quiet 1>/dev/null
	echo Giving webservice group permission to resstart kestrel-webapp.service...
	printf "%%webservice ALL=(root) NOPASSWD:/usr/bin/systemctl restart kestrel-webapp.service\n" \
	| tee /etc/sudoers.d/restart_kestrel_webapp 1>/dev/null
	echo Done creating kestrel-webapp service.

	echo Adding MongoDB repo...
	printf "[mongodb-org-4.2]
name=MongoDB Repository
baseurl=https://repo.mongodb.org/yum/redhat/\$releasever/mongodb-org/4.2/x86_64/
gpgcheck=1
enabled=1
gpgkey=https://www.mongodb.org/static/pgp/server-4.2.asc" | tee /etc/yum.repos.d/mongodb-org-4.2.repo 1>/dev/null
	echo Done adding MongoDB repo.
	
	echo Installing MongoDB...
	dnf install -q -y mongodb-org
	echo Configuring mongod to start on boot...
	systemctl enable mongod --quiet 1>/dev/null
	echo Done installing MongoDB.
	
	echo Binding MongoDB to all IPs...
	sed -i "s/bindIp: 127.0.0.1/bindIp: 0.0.0.0/" /etc/mongod.conf
	echo Done updating MongoDB IP binding.
	
	echo Configuring MongoDB SELinux policies...
	# Allow MongoDB to do some stuff, I guess?
	printf "module mongodb_cgroup_memory 1.0;

require {
    type cgroup_t;
    type mongod_t;
    class dir search;
    class file { getattr open read };
}

#============= mongod_t ==============
allow mongod_t cgroup_t:dir search;
allow mongod_t cgroup_t:file { getattr open read };" | tee ~/mongodb_cgroup_memory.te 1>/dev/null
	checkmodule -M -m -o ~/mongodb_cgroup_memory.mod ~/mongodb_cgroup_memory.te
	semodule_package -o ~/mongodb_cgroup_memory.pp -m ~/mongodb_cgroup_memory.mod
	semodule -i ~/mongodb_cgroup_memory.pp
	rm ~/mongodb_cgroup_memory.mod
	
	# Suppress some FTDC warnings (a fix is in the works on Fedora's side)
	printf "module mongodb_proc_net 1.0;

require {
    type proc_net_t;
    type mongod_t;
    class file { open read };
}

#============= mongod_t ==============
allow mongod_t proc_net_t:file { open read };" | tee ~/mongodb_proc_net.te 1>/dev/null
	checkmodule -M -m -o ~/mongodb_proc_net.mod ~/mongodb_proc_net.te
	semodule_package -o ~/mongodb_proc_net.pp -m ~/mongodb_proc_net.mod
	sudo semodule -i ~/mongodb_proc_net.pp
	rm ~/mongodb_proc_net.mod
	echo Done configuring MongoDB SELinux policies.
	
	echo Creating /data/db/ for MongoDB...
	mkdir -p /data/db/
	chown mongod:mongod /data/db/
	echo Done creating /data/db/.
	
	echo Starting mongod...
	systemctl start mongod --quiet
	echo Mongod started.
	
	echo Opening firewall ports for Apache and MongoDB...
	firewall-cmd --zone=public --add-service=http --permanent 1>/dev/null
	firewall-cmd --zone=public --add-service=https --permanent 1>/dev/null
	firewall-cmd --zone=public --add-service=mongodb --permanent 1>/dev/null
	firewall-cmd --reload 1>/dev/null
	echo Done opening firewall ports.
	
	echo Done with install
	echo Put build at /var/www/kestrel_webapp
	echo Start web server with restart, or start httpd and kestrel-webapp
fi
