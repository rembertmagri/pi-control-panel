# Pi Control Panel

Web control panel for Raspberry Pi 4 implemented on Angular and  .NET Core using GraphQL as API and EF Core as ORM. Allows easy overclocking, killing processes, rebooting and shutting down the Pi. It also provides real-time access to system information such as temperature, memory and disk usage, CPU load and network status.

Login | Dashboard | Real-Time Chart | Real-Time Chart (overclocking results)
------------ | ------------- | ------------- | -------------
![login](https://user-images.githubusercontent.com/30979154/82757722-630fb480-9db0-11ea-81f4-a88b3de05270.png) | ![dashboard](https://user-images.githubusercontent.com/30979154/82757721-630fb480-9db0-11ea-96e4-cdf52010dba8.png) | ![real-time chart](https://user-images.githubusercontent.com/30979154/82757720-62771e00-9db0-11ea-954d-35db3058d4ef.png) | ![overclocking results](https://user-images.githubusercontent.com/30979154/82757723-630fb480-9db0-11ea-8589-08743053dee1.png)

## Installing on Raspberry Pi

### From the private Debian Package Repository
Run the following command:
````bash
sh <(wget -qO- https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/install.sh)
````
It will add the private Debian package repository to the list and install the package.

### Manually from the Debian Package
1. Download the [latest release](https://github.com/rembertmagri/pi-control-panel/releases/latest)
2. Install the package
````bash
sudo apt install ./pi-control-panel_VERSION_armhf.deb
````
or (if running on Raspberry Pi OS 64)
````bash
sudo apt install ./pi-control-panel_VERSION_arm64.deb
````

## Running on Raspberry Pi
After installing, access http://localhost:8080 from the Pi or http://<<ip_of_raspberry_pi>>:8080 from another machine.

### Changing the default port
If port 8080 is already in use during installation, the application will run on the next available port. The port can also be changed manually at any time by editing /opt/picontrolpanel/Configuration/appsettings.json .

## Development

Check the [Wiki](https://github.com/rembertmagri/pi-control-panel/wiki) for documentation for developers
