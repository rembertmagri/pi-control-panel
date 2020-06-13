#!/bin/bash

gpg --list-keys F4109F88DD8DCD60 &> /dev/null
if [ $? -ne 0 ]; then
    wget -qO - https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo/PUBLIC.KEY | sudo apt-key add -
fi

grep -Fxq "deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main" /etc/apt/sources.list
if [ $? -ne 0 ]; then
    echo 'deb https://raw.githubusercontent.com/rembertmagri/pi-control-panel/master/debian_repo buster main' | sudo tee -a /etc/apt/sources.list
fi

sudo apt-get update
sudo apt-get install pi-control-panel
