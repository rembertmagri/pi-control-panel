#!/bin/bash

grep -Fxq "deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main" /etc/apt/sources.list
if [ $? -ne 0 ]; then
    wget -qO - https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo/PUBLIC.KEY | apt-key add -
    echo 'deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main' | tee -a /etc/apt/sources.list
else
    apt-get update
    apt-get install pi-control-panel
fi
