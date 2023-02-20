echo "Application service installer"

if [ "$EUID" -ne 0 ]
  then echo "This script must be run as root"
  exit
fi

user=$(whoami)

while getopts u: flag
do
    case "${flag}" in
        u) user=${OPTARG};;
    esac
done

echo $user

destination=/etc/systemd/system/sample-app-launcher.service
> $destination
cat <<EOT >> $destination
[Unit]
Description=Example launcher for another application

[Service]
WorkingDirectory=$(pwd)/launcher
ExecStart=/usr/bin/dotnet run
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=application-launcher-example
User=${user}
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=PORT=$1
Environment=APP_PORT=$2

[Install]
WantedBy=multi-user.target
EOT

echo "Enabling service"

systemctl enable sample-app-launcher.service

echo "Starting"

systemctl start sample-app-launcher.service
systemctl status sample-app-launcher.service

echo "Finished"