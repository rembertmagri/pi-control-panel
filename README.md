# Pi Control Panel

Web control panel for Raspberry Pi 4 implemented on Angular and  .NET Core using GraphQL as API and EF Core as ORM. Allows easy overclocking, killing processes, rebooting and shutting down the Pi. It also provides real-time access to system information such as temperature, memory and disk usage, CPU load and network status.

Login | Dashboard | Real-Time Chart | Real-Time Chart (overclocking results)
------------ | ------------- | ------------- | -------------
![login](https://user-images.githubusercontent.com/30979154/82757722-630fb480-9db0-11ea-81f4-a88b3de05270.png) | ![dashboard](https://user-images.githubusercontent.com/30979154/82757721-630fb480-9db0-11ea-96e4-cdf52010dba8.png) | ![real-time chart](https://user-images.githubusercontent.com/30979154/82757720-62771e00-9db0-11ea-954d-35db3058d4ef.png) | ![overclocking results](https://user-images.githubusercontent.com/30979154/82757723-630fb480-9db0-11ea-8589-08743053dee1.png)

## Installing on Raspberry Pi

### From the private Debian Package Repository
1. Add the private Debian package repository to the list
````bash
wget -qO - https://raw.githubusercontent.com/rembertmagri/debian/master/PUBLIC.KEY | sudo apt-key add -
echo 'deb https://raw.githubusercontent.com/rembertmagri/debian/master buster main' | sudo tee -a /etc/apt/sources.list
sudo apt-get update
````
2. Install the package
````bash
sudo apt-get install pi-control-panel
````

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

## Development

Check the [Wiki](https://github.com/rembertmagri/pi-control-panel/wiki/Development) for documentation for developers
