#!/bin/bash

grep -Fxq "deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main" /etc/apt/sources.list
if [ $? -ne 0 ]; then
    wget -qO - https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo/PUBLIC.KEY | sudo apt-key add -
    echo 'deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main' | sudo tee -a /etc/apt/sources.list
    sudo apt-get update
    sudo apt-get install pi-control-panel
else
    echo 'Pi Control Panel is already installed. Run "apt-get update" and "apt-get upgrade" to get the latest version.'
fi
