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

function configazagent
{
	su - azagent -s "/bin/bash" -c "cd /etc/azagent/ && ./config.sh"
	owd=$(pwd)
	cd /etc/azagent/
	./svc.sh install azagent
	newsvc=$(ls /etc/systemd/system/ | grep vsts*)
	cat "/etc/systemd/system/$newsvc" > /etc/systemd/system/azagent.service
	systemctl --quiet stop $newsvc
	systemctl --quiet disable $newsvc
	rm -f "/etc/systemd/system/$newsvc" 1>/dev/null
	systemctl --quiet daemon-reload
	systemctl --quiet enable azagent
	systemctl --quiet start azagent
	cd $owd
	echo
	echo
	echo Azure Agent service: azagent.service
	echo
	echo
	echo Configuration is done!
}

function disable_selinux
{
	sed -i "s/SELINUX=enforcing/SELINUX=disabled/" /etc/selinux/config
	
	while $true; do
        echo -n "CentoOS needs to reboot to finish disabling SELinux. Do it now? (Y/N): "
        read resp
        if [[ "$resp" == "Y" || "$resp" == "y" ]]; then
                shutdown -r now
                exit 0
        elif [[ "$resp" == "N" || "$resp" == "n" ]]; then
                echo "Reboot the server before running this script again."
				exit 0
        else
			echo -n "CentoOS needs to reboot to finish disabling SELinux. Do it now? (Y/N): "
        fi
	done
}

# https://unix.stackexchange.com/questions/28791/prompt-for-sudo-password-and-programmatically-elevate-privilege-in-bash-script
if [ $UID -ne 0 ]; then
	echo This script was not executed as root.
	echo Rerunning the script with sudo...
	exec sudo bash "$0" "$@";
else

	if [ "$(sestatus)" != "SELinux status:                 disabled" ]; then
		if [ "$(sestatus | grep 'Current mode:')" != "Current mode:                   permissive" ]; then
			while $true; do
				echo -n "SELinux is enabled in Enforcing mode. Disable it now? (Y/N): "
				read resp
				if [[ "$resp" == "Y" || "$resp" == "y" ]]; then
						disable_selinux
						exit 0
				elif [[ "$resp" == "N" || "$resp" == "n" ]]; then
						echo "Script cannot continue with SELinux in Enforcing mode. Disable it before running this script again."
						exit 1
				else
					echo -n "SELinux is enabled in Enforcing mode. Disable it now? (Y/N): "
				fi
			done
		else
			echo SELinux is in Permissive mode
		fi
	else
		echo SELinux is disabled
	fi
	
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
	
	echo Making kestrel_webapp folder...
	mkdir -p /var/www/kestrel_webapp
	chown apache:webservice -R /var/www/kestrel_webapp/
	chmod 775 -R /var/www/kestrel_webapp/
	chmod g+s /var/www/kestrel_webapp
	
	echo Importing Microsoft .NET repo...
	rpm -U https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm 1>/dev/null
	echo Done importing repo.
	
	echo Installing .NET Core SDK dependencies...
	sudo dnf -q -y install lttng-ust libcurl openssl-libs krb5-libs libicu zlib compat-openssl10 git
	echo Done installing dependencies
	
	echo Installing .NET Core SDK...
	dnf install -q -y dotnet-sdk-3.1
	echo Done installing .NET Core SDK.
	
	echo Installing ASP.NET Core runtime...
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
User=azagent
Environment=ASPNETCORE_ENVIRONMENT=Development
TimeoutStopSec=90

[Install]
WantedBy=multi-user.target
" | tee /etc/systemd/system/kestrel-webapp.service 1>/dev/null
	echo Creating kestrel-webapp start script...
	mkdir -p /usr/local/bin/kestrel_webapp/
	printf "/usr/bin/dotnet /var/www/kestrel_webapp/$dllname" | tee /usr/local/bin/kestrel_webapp/start.sh 1>/dev/null
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
	systemctl --quiet enable mongod 1>/dev/null
	echo Done installing MongoDB.
	
	echo Binding MongoDB to all IPs...
	sed -i "s/bindIp: 127.0.0.1/bindIp: 0.0.0.0/" /etc/mongod.conf
	echo Done updating MongoDB IP binding.
	
	echo Creating /data/db/ for MongoDB...
	mkdir -p /data/db/
	chown mongod:mongod /data/db/
	echo Done creating /data/db/.
	
	echo Opening firewall ports for Apache and MongoDB...
	firewall-cmd --zone=public --add-service=http --permanent 1>/dev/null
	firewall-cmd --zone=public --add-service=https --permanent 1>/dev/null
	firewall-cmd --zone=public --add-service=mongodb --permanent 1>/dev/null
	firewall-cmd --reload 1>/dev/null
	echo Done opening firewall ports.
	
	echo Downloading and preparing Azure agent...
	useradd -m --shell /bin/false azagent
	echo Giving azagent ownership of /usr/local/bin/kestrel_webapp/start.sh
	chown azagent:azagent /usr/local/bin/kestrel_webapp/*
	cd /tmp/
	wget -q https://vstsagentpackage.azureedge.net/agent/2.164.3/vsts-agent-linux-x64-2.164.3.tar.gz
	mkdir -p /etc/azagent/
	tar --directory=/etc/azagent/ -xzf /tmp/vsts-agent-linux-x64-2.164.3.tar.gz
	rm /tmp/vsts-agent-linux-x64-2.164.3.tar.gz
	chown azagent:azagent -R /etc/azagent/
	mkdir -p /var/azagent/_work
	chown azagent:azagent -R /var/azagent/
	echo Adding azagent to webservice group...
	sudo usermod -aG webservice azagent
	echo Done downloading and preparing Azure agent.
	
	echo Running agent dependency script...
	bash /etc/azagent/bin/installdependencies.sh 1>/dev/null
	echo Done running dependency script.
	
	echo
	echo
	echo
	echo Done with install!!!
	echo
	echo
	echo
	echo To change the .dll the Kestrel web server runs, edit /etc/systemd/system/kestrel-webapp.service 
	echo Use work directory of /var/azagent/_work/ when configuring the agent
	echo Put build at /var/www/kestrel_webapp
	echo
	echo
	echo
	
	while $true; do
        echo -n "Would you like to configure the agent now? (Y/N): "
        read resp
        if [[ "$resp" == "Y" || "$resp" == "y" ]]; then
                configazagent
                break
        elif [[ "$resp" == "N" || "$resp" == "n" ]]; then
                break
        else
                echo -n "Would you like to configure the agent now? (Y/N): "
        fi
	done
	
	

fi
