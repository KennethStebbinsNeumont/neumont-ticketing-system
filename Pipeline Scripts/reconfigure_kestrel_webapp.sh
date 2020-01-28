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

if [ $UID -ne 0 ]; then
	echo This script was not executed as root.
	echo Rerunning the script with sudo...
	exec sudo bash "$0" "$@";
else
	systemctl --quiet stop kestrel-webapp.service
	systemctl --quiet disable kestrel-webapp.service
	printf "/usr/bin/dotnet /var/www/kestrel_webapp/$dllname" | tee /usr/local/bin/kestrel_webapp/start.sh 1>/dev/null
	chown kestrel:kestrel /usr/local/bin/kestrel_webapp/*
	chmod u+x /usr/local/bin/kestrel_webapp/start.sh
	systemctl --quiet enable kestrel-webapp.service
	echo Run systemctl start kestrel-webapp.service to start the service.
fi
