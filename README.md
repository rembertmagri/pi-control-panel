# Pi Control Panel

## Creating the Debian Package

1. Publish the PiControlPanel.Api.GraphQL project targeting ARM; this will publish the files into pi-control-panel/package/pi-control-panel_VERSION.SUBVERSION_armhf/opt/picontrolpanel
2. If building on Windows, copy the entire content of pi-control-panel/package to your Raspberry Pi or any other Linux machine and give execution permission to the following files
````bash
chmod +x pi-control-panel_VERSION.SUBVERSION_armhf/DEBIAN/preinst
chmod +x pi-control-panel_VERSION.SUBVERSION_armhf/DEBIAN/postinst
chmod +x pi-control-panel_VERSION.SUBVERSION_armhf/DEBIAN/prerm
chmod +x pi-control-panel_VERSION.SUBVERSION_armhf/DEBIAN/postrm
````
3. Rename directory pi-control-panel_VERSION.SUBVERSION_armhf to match the version of the packages. Example:
````bash
mv pi-control-panel_VERSION.SUBVERSION_armhf pi-control-panel_1.3_armhf
````
4. Build the Package
````bash
dpkg-deb --build pi-control-panel_1.3_armhf
````

## Running

### Running on Raspberry Pi from the Debian package
1. Download or copy the package
````bash
git clone https://github.com/rembertmagri/pi-control-panel-package.git
cd pi-control-panel-package/ 
````
2. Install the package
````bash
sudo apt install ./pi-control-panel_1.3_armhf.deb
````
3. Access http://<<ip_of_raspberry_pi>>:8080/
4. To uninstall:
````bash
sudo apt remove pi-control-panel
````

### Running on Raspberry Pi from the source code
1. Create the user to run the app and the log directory
````bash
sudo useradd -m picontrolpanel
sudo passwd picontrolpanel
sudo usermod -aG sudo picontrolpanel
echo 'picontrolpanel ALL=(ALL) NOPASSWD: ALL' | sudo EDITOR='tee -a' visudo
sudo usermod -aG video picontrolpanel
sudo mkdir /var/log/picontrolpanel
sudo chown picontrolpanel /var/log/picontrolpanel
````
2. Publish PiControlPanel.Api.GraphQL project targeting ARM and copy the files to /opt/picontrolpanel
3. Copy pi-control-panel\package\pi-control-panel_1.0_armhf\etc\systemd\system\picontrolpanel.service to /etc/systemd/system/picontrolpanel.service
4. Login as picontrolpanel
````bash
su - picontrolpanel
````
5. Give execution permission to the application
````bash
chmod +x PiControlPanel.Api.GraphQL
````
6. Run as process
````bash
export ASPNETCORE_URLS=http://+:8080
export ASPNETCORE_ENVIRONMENT=Production
./PiControlPanel.Api.GraphQL
````
6. Or run as service
````bash
sudo chmod 644 /etc/systemd/system/picontrolpanel.service
sudo systemctl enable picontrolpanel
sudo systemctl start picontrolpanel
````
7. Access http://<<ip_of_raspberry_pi>>:8080/

#### Available operations
To test the available operations directly, run the application as a process setting the environment to Development and accessing http://<<ip_of_raspberry_pi>>:8080/playground

##### Login Query

Query:
````graphql
query Login($userAccount: UserAccountInputType) {
  login(userAccount: $userAccount) {
    username
    jwt
    roles
  }
}
````

Query variables:
````graphql
{
  "userAccount": {
    "username": "pi",
    "password": "raspberry"
  }
}
````

##### RaspberryPi Query

Query:
````graphql
query RaspberryPi {
  raspberryPi {
    chipset {
      model
      revision
      serial
      version
    }
    cpu {
      cores
      model
      maxFrequency
      frequency {
        value
        dateTime
      }
      temperature {
        value
        dateTime
      }
      loadStatus {
        dateTime
        lastMinuteAverage
        last5MinutesAverage
        last15MinutesAverage
        kernelRealTime
        userRealTime
        totalRealTime
        processes {
          processId
          user
          priority
          niceValue
          totalMemory
          ram
          sharedMemory
          state
          cpuPercentage
          ramPercentage
          totalCpuTime
          command
        }
      }
    }
    disk {
      fileSystems {
        name
        total
        type
        status {
          fileSystemName
          available
          used
          dateTime
        }
      }
    }
    ram {
      total
      status {
        used
        free
        diskCache
        dateTime
      }
    }
    swapMemory {
      total
      status {
        used
        free
        dateTime
      }
    }
    gpu {
      memory
      frequency
    }
    os {
      name
      kernel
      hostname
      status {
        uptime
        dateTime
      }
    }
    network {
      networkInterfaces {
        name
        ipAddress
        subnetMask
        defaultGateway
        status {
          networkInterfaceName
          totalReceived
          receiveSpeed
          totalSent
          sendSpeed
          dateTime
        }
      }
    }
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu First Frequencies Query

Query:
````graphql
query CpuFrequencies($firstFrequencies: Int, $afterFrequencies: String) {
  raspberryPi {
    cpu {
      frequencies(first: $firstFrequencies, after: $afterFrequencies) {
      items {
          value
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstFrequencies": 10,
  "afterFrequencies": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Last Frequencies Query

Query:
````graphql
query CpuFrequencies($lastFrequencies: Int, $beforeFrequencies: String) {
  raspberryPi {
    cpu {
      frequencies(last: $lastFrequencies, before: $beforeFrequencies) {
      items {
          value
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastFrequencies": 10,
  "beforeFrequencies": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Frequency Subscription

Query:
````graphql
subscription CpuFrequency {
    cpuFrequency {
      value
      dateTime
    }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu First Temperatures Query

Query:
````graphql
query CpuTemperatures($firstTemperatures: Int, $afterTemperatures: String) {
  raspberryPi {
    cpu {
      temperatures(first: $firstTemperatures, after: $afterTemperatures) {
      items {
          value
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstTemperatures": 10,
  "afterTemperatures": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Last Temperatures Query

Query:
````graphql
query CpuTemperatures($lastTemperatures: Int, $beforeTemperatures: String) {
  raspberryPi {
    cpu {
      temperatures(last: $lastTemperatures, before: $beforeTemperatures) {
      items {
          value
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastTemperatures": 10,
  "beforeTemperatures": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Temperature Subscription

Query:
````graphql
subscription CpuTemperature {
    cpuTemperature {
      value
      dateTime
    }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu First Load Statuses Query

Query:
````graphql
query CpuLoadStatuses($firstLoadStatuses: Int, $afterLoadStatuses: String) {
  raspberryPi {
    cpu {
      loadStatuses(first: $firstLoadStatuses, after: $afterLoadStatuses) {
        items {
          dateTime
          lastMinuteAverage
          last5MinutesAverage
          last15MinutesAverage
          kernelRealTime
          userRealTime
          totalRealTime
          processes {
            processId
            user
            priority
            niceValue
            totalMemory
            ram
            sharedMemory
            state
            cpuPercentage
            ramPercentage
            totalCpuTime
            command
          }
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstLoadStatuses": 10,
  "afterLoadStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Last Load Statuses Query

Query:
````graphql
query CpuLoadStatuses($lastLoadStatuses: Int, $beforeLoadStatuses: String) {
  raspberryPi {
    cpu {
      loadStatuses(last: $lastLoadStatuses, before: $beforeLoadStatuses) {
        items {
          dateTime
          lastMinuteAverage
          last5MinutesAverage
          last15MinutesAverage
          kernelRealTime
          userRealTime
          totalRealTime
          processes {
            processId
            user
            priority
            niceValue
            totalMemory
            ram
            sharedMemory
            state
            cpuPercentage
            ramPercentage
            totalCpuTime
            command
          }
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastLoadStatuses": 10,
  "beforeLoadStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Cpu Load Status Subscription

Query:
````graphql
subscription CpuLoadStatus {
  cpuLoadStatus {
    dateTime
    lastMinuteAverage
    last5MinutesAverage
    last15MinutesAverage
    kernelRealTime
    userRealTime
    totalRealTime
    processes {
      processId
      user
      priority
      niceValue
      totalMemory
      ram
      sharedMemory
      state
      cpuPercentage
      ramPercentage
      totalCpuTime
      command
    }
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### RAM First Statuses Query

Query:
````graphql
query RamStatuses($firstMemoryStatuses: Int, $afterMemoryStatuses: String) {
  raspberryPi {
    ram {
      statuses(first: $firstMemoryStatuses, after: $afterMemoryStatuses) {
        items {
          used
          free
          diskCache
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstMemoryStatuses": 10,
  "afterMemoryStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### RAM Last Statuses Query

Query:
````graphql
query RamStatuses($lastMemoryStatuses: Int, $beforeMemoryStatuses: String) {
  raspberryPi {
    ram {
      statuses(last: $lastMemoryStatuses, before: $beforeMemoryStatuses) {
        items {
          used
          free
          diskCache
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastMemoryStatuses": 10,
  "beforeMemoryStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### RAM Status Subscription

Query:
````graphql
subscription RamStatus {
  ramStatus {
    used
    free
    diskCache
    dateTime
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Swap Memory First Statuses Query

Query:
````graphql
query SwapMemoryStatuses($firstMemoryStatuses: Int, $afterMemoryStatuses: String) {
  raspberryPi {
    swapMemory {
      statuses(first: $firstMemoryStatuses, after: $afterMemoryStatuses) {
        items {
          used
          free
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstMemoryStatuses": 10,
  "afterMemoryStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Swap Memory Last Statuses Query

Query:
````graphql
query SwapMemoryStatuses($lastMemoryStatuses: Int, $beforeMemoryStatuses: String) {
  raspberryPi {
    swapMemory {
      statuses(last: $lastMemoryStatuses, before: $beforeMemoryStatuses) {
        items {
          used
          free
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastMemoryStatuses": 10,
  "beforeMemoryStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Swap Memory Status Subscription

Query:
````graphql
subscription SwapMemoryStatus {
  swapMemoryStatus {
    used
    free
    dateTime
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### File System First Statuses Query

Query:
````graphql
query FileSystemStatuses($name: String!, $firstFileSystemStatuses: Int, $afterFileSystemStatuses: String) {
  raspberryPi {
    disk {
      fileSystem(name: $name) {
        statuses(first: $firstFileSystemStatuses, after: $afterFileSystemStatuses) {
      	  items {
      	    fileSystemName
            available
            used
            dateTime
          }
          pageInfo {
            startCursor
            hasPreviousPage
            endCursor
            hasNextPage
          }
          totalCount
        }
      }
    }
  }
}
````

Query variables:
````graphql
{
  "name": "/dev/root",
  "firstFileSystemStatuses": 10,
  "afterFileSystemStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### File System Last Statuses Query

Query:
````graphql
query FileSystemStatuses($name: String!, $lastFileSystemStatuses: Int, $beforeFileSystemStatuses: String) {
  raspberryPi {
    disk {
      fileSystem(name: $name) {
        statuses(last: $lastFileSystemStatuses, before: $beforeFileSystemStatuses) {
      	  items {
            fileSystemName
            available
            used
            dateTime
          }
          pageInfo {
            startCursor
            hasPreviousPage
            endCursor
            hasNextPage
          }
          totalCount
        }
      }
    }
  }
}
````

Query variables:
````graphql
{
  "name": "/dev/root",
  "lastFileSystemStatuses": 10,
  "beforeFileSystemStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### File System Status Subscription

Query:
````graphql
subscription FileSystemStatus($fileSystemName: String!) {
  fileSystemStatus(fileSystemName: $fileSystemName) {
    fileSystemName
    available
    used
    dateTime
  }
}
````

Query variables:
````graphql
{
  "fileSystemName": "/dev/root"
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### OS First Statuses Query

Query:
````graphql
query OsStatuses($firstOsStatuses: Int, $afterOsStatuses: String) {
  raspberryPi {
    os {
      statuses(first: $firstOsStatuses, after: $afterOsStatuses) {
        items {
          uptime
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "firstOsStatuses": 10,
  "afterOsStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### OS Last Statuses Query

Query:
````graphql
query OsStatuses($lastOsStatuses: Int, $beforeOsStatuses: String) {
  raspberryPi {
    os {
      statuses(last: $lastOsStatuses, before: $beforeOsStatuses) {
        items {
          uptime
          dateTime
        }
        pageInfo {
          startCursor
          hasPreviousPage
          endCursor
          hasNextPage
        }
        totalCount
      }
    }
  }
}
````

Query variables:
````graphql
{
  "lastOsStatuses": 10,
  "beforeOsStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### OS Status Subscription

Query:
````graphql
subscription OsStatus {
  os {
    uptime
    dateTime
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Network Interface First Statuses Query

Query:
````graphql
query NetworkInterfaceStatuses($name: String!, $firstNetworkInterfaceStatuses: Int, $afterNetworkInterfaceStatuses: String) {
  raspberryPi {
    network {
      networkInterface(name: $name) {
        statuses(first: $firstNetworkInterfaceStatuses, after: $afterNetworkInterfaceStatuses) {
      	  items {
            networkInterfaceName
            dateTime
            receiveSpeed
            sendSpeed
            totalReceived
            totalSent
          }
          pageInfo {
            startCursor
            hasPreviousPage
            endCursor
            hasNextPage
          }
          totalCount
        }
      }
    }
  }
}
````

Query variables:
````graphql
{
  "name": "wlan0",
  "firstNetworkInterfaceStatuses": 10,
  "afterNetworkInterfaceStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Network Interface Last Statuses Query

Query:
````graphql
query NetworkInterfaceStatuses($name: String!, $lastNetworkInterfaceStatuses: Int, $beforeNetworkInterfaceStatuses: String) {
  raspberryPi {
    network {
      networkInterface(name: $name) {
        statuses(last: $lastNetworkInterfaceStatuses, before: $beforeNetworkInterfaceStatuses) {
      	  items {
            networkInterfaceName
            dateTime
            receiveSpeed
            sendSpeed
            totalReceived
            totalSent
          }
          pageInfo {
            startCursor
            hasPreviousPage
            endCursor
            hasNextPage
          }
          totalCount
        }
      }
    }
  }
}
````

Query variables:
````graphql
{
  "name": "eth0",
  "lastNetworkInterfaceStatuses": 10,
  "beforeNetworkInterfaceStatuses": "ca23bf..."
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Network Interface Status Subscription

Query:
````graphql
subscription NetworkInterfaceStatus($networkInterfaceName: String!) {
  networkInterfaceStatus(networkInterfaceName: $networkInterfaceName) {
    networkInterfaceName
    receiveSpeed
    sendSpeed
    totalReceived
    totalSent
    dateTime
  }
}
````

Query variables:
````graphql
{
  "networkInterfaceName": "wlan0"
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Refresh Token Query

Query:
````graphql
query RefreshToken {
  refreshToken {
    username
    jwt
    roles
  }
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Reboot Mutation

Query:
````graphql
mutation Reboot {
  reboot
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Shutdown Mutation

Query:
````graphql
mutation Shutdown {
  shutdown
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Update Mutation

Query:
````graphql
mutation Update {
  update
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Kill Process Mutation

Query:
````graphql
mutation Kill($processId: Int!) {
  kill(processId: $processId)
}
````

Mutation variables:
````graphql
{
  "processId": 4539
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

##### Overclock Mutation

Query:
````graphql
mutation overclock($cpuMaxFrequencyLevel: CpuMaxFrequencyLevel!) {
  overclock(cpuMaxFrequencyLevel: $cpuMaxFrequencyLevel)
}
````

Mutation variables:
````graphql
{
  "cpuMaxFrequencyLevel": "MAXIMUM"
}
````

HTTP Headers:
````graphql
{
  "Authorization" : "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
}
````

### Running on Docker

#### Docker on Raspberry Pi
````bash
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
rm get-docker.sh
sudo usermod -aG docker picontrolpanel
sudo apt-get install docker-compose
apt-get -y install git
git clone https://github.com/rembertmagri/pi-control-panel.git
cd pi-control-panel/Docker/
docker-compose -f docker-compose.pi.yml build
docker-compose -f docker-compose.pi.yml up -d
````
Access http://<<ip_of_raspberry_pi>>:8081/

#### Docker outside Raspberry Pi
1. Open terminal inside the solution's /Docker directory
2. Build the image
````command
docker-compose build
````
3. Run the container
````command
docker-compose up -d
````
4. Access http://localhost:8080/

#### Available fields when running on Docker

* Login Query
* RaspberryPi Query
````graphql
  raspberryPi {
    disk {
      fileSystems {
        name
        total
        type
        status {
          fileSystemName
          available
          used
          dateTime
        }
      }
    }
    ram {
      total
      status {
        used
        free
        diskCache
        dateTime
      }
    }
    swapMemory {
      total
      status {
        used
        free
        dateTime
      }
    }
  }
````
* All File System Status queries and subscription
* All RAM and Swap Memory Status queries and subscription
* Refresh Token Query
* Shutdown Mutation (Returns true, but doesn't actually shut the container down)
