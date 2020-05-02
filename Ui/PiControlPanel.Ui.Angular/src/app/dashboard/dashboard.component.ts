import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RaspberryPiService } from '../shared/services/raspberry-pi.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { 
  IRaspberryPi,
  ICpuFrequency,
  ICpuTemperature,
  ICpuLoadStatus,
  IMemoryStatus, 
  IRandomAccessMemoryStatus, 
  INetworkInterfaceStatus} from '../shared/interfaces/raspberry-pi';
import { AuthService } from '../shared/services/auth.service';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import {
  remove,
  orderBy,
  map,
  isNil,
  first,
  startsWith,
  endsWith,
  trimEnd,
  max,
  min,
  take as _take,
  difference,
  forEach,
  invoke,
  includes,
  fill,
  isEmpty,
  find } from 'lodash';
import { RealTimeModalComponent } from './modal/real-time-modal.component';
import { CpuFrequencyService } from 'src/app/shared/services/cpu-frequency.service';
import { CpuTemperatureService } from 'src/app/shared/services/cpu-temperature.service';
import { CpuLoadStatusService } from 'src/app/shared/services/cpu-load-status.service';
import { RamStatusService } from 'src/app/shared/services/ram-status.service';
import { SwapMemoryStatusService } from 'src/app/shared/services/swap-memory-status.service';
import { DiskStatusService } from '../shared/services/disk-status.service';
import { OsStatusService } from '../shared/services/os-status.service';
import { NetworkInterfaceStatusService } from '../shared/services/network-interface-status.service';
import { CpuMaxFrequencyLevel } from '../shared/constants/cpu-max-frequency-level';
import { ChartData } from '../shared/constants/chart-data';
import { MAX_CHART_VISIBLE_ITEMS } from '../shared/constants/consts';

@Component({
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  raspberryPi: IRaspberryPi;
  errorMessage: string;
  modalRef: BsModalRef;

  subscribedToNewCpuFrequencies: boolean;
  subscribedToNewCpuTemperatures: boolean;
  subscribedToNewCpuLoadStatuses: boolean;
  subscribedToNewRamStatuses: boolean;
  subscribedToNewSwapMemoryStatuses: boolean;
  subscribedToNewDiskStatuses: boolean;
  subscribedToNewOsStatuses: boolean;
  subscribedToNewNetworkInterfaceStatuses: boolean[];

  cpuFrequencyBehaviorSubjectSubscription: Subscription;
  cpuTemperatureBehaviorSubjectSubscription: Subscription;
  cpuLoadStatusBehaviorSubjectSubscription: Subscription;
  ramStatusBehaviorSubjectSubscription: Subscription;
  swapMemoryStatusBehaviorSubjectSubscription: Subscription;
  diskStatusBehaviorSubjectSubscription: Subscription;
  osStatusBehaviorSubjectSubscription: Subscription;
  networkInterfaceStatusBehaviorSubjectSubscriptions: Subscription[];

  isSuperUser: boolean;
  refreshTokenPeriodicallySubscription: Subscription;
  CpuMaxFrequencyLevel = CpuMaxFrequencyLevel;

  readonly MAX_CHART_VISIBLE_ITEMS = MAX_CHART_VISIBLE_ITEMS;
  selectedChartItems: string [];
  unselectedChartItems: string [];

  constructor(private _route: ActivatedRoute,
    private router: Router,
    private modalService: BsModalService,
    private raspberryPiService: RaspberryPiService,
    private authService: AuthService,
    private cpuFrequencyService: CpuFrequencyService,
    private cpuTemperatureService: CpuTemperatureService,
    private cpuLoadStatusService: CpuLoadStatusService,
    private ramStatusService: RamStatusService,
    private swapMemoryStatusService: SwapMemoryStatusService,
    private diskStatusService: DiskStatusService,
    private osStatusService: OsStatusService,
    private networkInterfaceStatusService: NetworkInterfaceStatusService) { }

  ngOnInit() {
    this.raspberryPi = this._route.snapshot.data['raspberryPi'];
    this.isSuperUser = this.authService.isSuperUser();

    const chartDataNames = map(ChartData, 'name');
    this.selectedChartItems = _take(chartDataNames, MAX_CHART_VISIBLE_ITEMS);
    this.unselectedChartItems = difference(chartDataNames, this.selectedChartItems);

    this.refreshTokenPeriodicallySubscription = this.authService.refreshTokenPeriodically()
      .subscribe(
        result => {
          console.log(result ? `Token refreshed @ ${new Date()}` : "Failed to refresh token");
        },
        error => this.errorMessage = <any>error
      );

    this.subscribedToNewCpuFrequencies = false;
    this.cpuFrequencyBehaviorSubjectSubscription = this.cpuFrequencyService.getLastCpuFrequencies()
      .subscribe(
        result => {
          this.raspberryPi.cpu.frequency = first(result.items);
          this.raspberryPi.cpu.frequencies = result.items;
          if(!isNil(this.modalRef) && includes(this.selectedChartItems, ChartData[0].name)) {
            this.modalRef.content.chartData[0].series = this.getOrderedAndMappedCpuNormalizedFrequencies();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewCpuFrequencies) {
            this.cpuFrequencyService.subscribeToNewCpuFrequencies();
            this.subscribedToNewCpuFrequencies = true;
          }
        },
        error => this.errorMessage = <any>error
      );

    this.subscribedToNewCpuTemperatures = false;
    this.cpuTemperatureBehaviorSubjectSubscription = this.cpuTemperatureService.getLastCpuTemperatures()
      .subscribe(
        result => {
          this.raspberryPi.cpu.temperature = first(result.items);
          this.raspberryPi.cpu.temperatures = result.items;
          if(!isNil(this.modalRef) && includes(this.selectedChartItems, ChartData[1].name)) {
            this.modalRef.content.chartData[1].series = this.getOrderedAndMappedCpuTemperatures();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewCpuTemperatures) {
            this.cpuTemperatureService.subscribeToNewCpuTemperatures();
            this.subscribedToNewCpuTemperatures = true;
          }
        },
        error => this.errorMessage = <any>error
      );

    this.subscribedToNewCpuLoadStatuses = false;
    this.cpuLoadStatusBehaviorSubjectSubscription = this.cpuLoadStatusService.getLastCpuLoadStatuses()
      .subscribe(
        result => {
          this.raspberryPi.cpu.loadStatus = first(result.items);
          this.raspberryPi.cpu.loadStatuses = result.items;
          if(!isNil(this.modalRef) && includes(this.selectedChartItems, ChartData[2].name)) {
            this.modalRef.content.chartData[2].series = this.getOrderedAndMappedCpuLoadStatuses();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewCpuLoadStatuses) {
            this.cpuLoadStatusService.subscribeToNewCpuLoadStatuses();
            this.subscribedToNewCpuLoadStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );

    this.subscribedToNewRamStatuses = false;
    this.ramStatusBehaviorSubjectSubscription = this.ramStatusService.getLastMemoryStatuses()
      .subscribe(
        result => {
          this.raspberryPi.ram.status = first(result.items);
          this.raspberryPi.ram.statuses = result.items;
          if(!isNil(this.modalRef) && includes(this.selectedChartItems, ChartData[3].name)) {
            this.modalRef.content.chartData[3].series = this.getOrderedAndMappedRamStatuses();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewRamStatuses) {
            this.ramStatusService.subscribeToNewMemoryStatuses();
            this.subscribedToNewRamStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );

    this.subscribedToNewSwapMemoryStatuses = false;
    this.swapMemoryStatusBehaviorSubjectSubscription = this.swapMemoryStatusService.getLastMemoryStatuses()
      .subscribe(
        result => {
          this.raspberryPi.swapMemory.status = first(result.items);
          this.raspberryPi.swapMemory.statuses = result.items;
          if(!isNil(this.modalRef) && includes(this.selectedChartItems, ChartData[4].name)) {
            this.modalRef.content.chartData[4].series = this.getOrderedAndMappedSwapMemoryStatuses();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewSwapMemoryStatuses) {
            this.swapMemoryStatusService.subscribeToNewMemoryStatuses();
            this.subscribedToNewSwapMemoryStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );
    
    this.subscribedToNewDiskStatuses = false;
    this.diskStatusBehaviorSubjectSubscription = this.diskStatusService.getLastDiskStatuses()
      .subscribe(
        result => {
          this.raspberryPi.disk.status = first(result.items);
          this.raspberryPi.disk.statuses = result.items;
          if(!this.subscribedToNewDiskStatuses) {
            this.diskStatusService.subscribeToNewDiskStatuses();
            this.subscribedToNewDiskStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );
    
    this.subscribedToNewOsStatuses = false;
    this.osStatusBehaviorSubjectSubscription = this.osStatusService.getLastOsStatuses()
      .subscribe(
        result => {
          this.raspberryPi.os.status = first(result.items);
          this.raspberryPi.os.statuses = result.items;
          if(!this.subscribedToNewOsStatuses) {
            this.osStatusService.subscribeToNewOsStatuses();
            this.subscribedToNewOsStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );
    
    const numberOfNetworkInterfaces = this.raspberryPi.network.networkInterfaces.length;
    this.subscribedToNewNetworkInterfaceStatuses = fill(Array(numberOfNetworkInterfaces), false);
    this.networkInterfaceStatusBehaviorSubjectSubscriptions = fill(Array(numberOfNetworkInterfaces), null);
    for(const networkInterface of this.raspberryPi.network.networkInterfaces) {
      const interfaceName = networkInterface.name;

      this.unselectedChartItems.push(`Network ${interfaceName} Rx (B/s)`);
      this.unselectedChartItems.push(`Network ${interfaceName} Tx (B/s)`);

      this.networkInterfaceStatusBehaviorSubjectSubscriptions[interfaceName] =
        this.networkInterfaceStatusService.getLastNetworkInterfaceStatuses(interfaceName)
          .subscribe(
            result => {
              networkInterface.status = first(result.items);
              networkInterface.statuses = result.items;
              const index = this.raspberryPi.network.networkInterfaces.indexOf(networkInterface);
              if(!isNil(this.modalRef)) {
                if(includes(this.selectedChartItems, `Network ${interfaceName} Rx (B/s)`)) {
                  this.modalRef.content.chartData[5+2*index].series = this.getOrderedAndMappedRxNetworkInterfaceNormalizedStatuses(interfaceName);
                  this.modalRef.content.chartData = [...this.modalRef.content.chartData];
                }
                if(includes(this.selectedChartItems, `Network ${interfaceName} Tx (B/s)`)) {
                  this.modalRef.content.chartData[5+2*index+1].series = this.getOrderedAndMappedTxNetworkInterfaceNormalizedStatuses(interfaceName);
                  this.modalRef.content.chartData = [...this.modalRef.content.chartData];
                }
              }
              if(!this.subscribedToNewNetworkInterfaceStatuses[interfaceName]) {
                this.networkInterfaceStatusService.subscribeToNewNetworkInterfaceStatuses(interfaceName);
                this.subscribedToNewNetworkInterfaceStatuses[interfaceName] = true;
              }
            },
            error => this.errorMessage = <any>error
          );
    }
  }

  ngOnDestroy(): void {
    if (!isNil(this.refreshTokenPeriodicallySubscription)) {
      this.refreshTokenPeriodicallySubscription.unsubscribe();
    }
    if (!isNil(this.cpuFrequencyBehaviorSubjectSubscription)) {
      this.cpuFrequencyBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.cpuTemperatureBehaviorSubjectSubscription)) {
      this.cpuTemperatureBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.cpuLoadStatusBehaviorSubjectSubscription)) {
      this.cpuLoadStatusBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.ramStatusBehaviorSubjectSubscription)) {
      this.ramStatusBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.swapMemoryStatusBehaviorSubjectSubscription)) {
      this.swapMemoryStatusBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.diskStatusBehaviorSubjectSubscription)) {
      this.diskStatusBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isNil(this.osStatusBehaviorSubjectSubscription)) {
      this.osStatusBehaviorSubjectSubscription.unsubscribe();
    }
    if (!isEmpty(this.networkInterfaceStatusBehaviorSubjectSubscriptions)) {
      for(const networkInterfaceStatusBehaviorSubjectSubscription of this.networkInterfaceStatusBehaviorSubjectSubscriptions) {
        networkInterfaceStatusBehaviorSubjectSubscription.unsubscribe();
      }
    }
  }

  openModal() {
    this.modalRef = this.modalService.show(
      RealTimeModalComponent,
      {
        class: 'modal-xl'
      });
    this.modalRef.content.chartData = [];
    forEach(ChartData, (chartDataItem) => {
      this.modalRef.content.chartData.push(
        {
          name: chartDataItem.name,
          series: includes(this.selectedChartItems, chartDataItem.name) ?
            invoke(this, chartDataItem.seriesMethod) : []
        }
      );
    });
    forEach(this.raspberryPi.network.networkInterfaces, (networkInterface) => {
      const interfaceName = networkInterface.name;
      this.modalRef.content.chartData.push(
        {
          name: `Network ${interfaceName} Rx (B/s)`,
          series: includes(this.selectedChartItems, `Network ${interfaceName} Rx (B/s)`) ?
            this.getOrderedAndMappedRxNetworkInterfaceNormalizedStatuses(interfaceName) : []
        }
      );
      this.modalRef.content.chartData.push(
        {
          name: `Network ${interfaceName} Tx (B/s)`,
          series: includes(this.selectedChartItems, `Network ${interfaceName} Tx (B/s)`) ?
            this.getOrderedAndMappedTxNetworkInterfaceNormalizedStatuses(interfaceName) : []
        }
      );
    });
    this.modalRef.content.chartData = [...this.modalRef.content.chartData];
  }

  reboot() {
    this.raspberryPiService.rebootRaspberryPi()
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert('Rebooting...');
          this.logout();
        }
        else {
          alert('Error');
        }
      },
      error => this.errorMessage = <any>error
    );
  }
  
  shutdown() {
    this.raspberryPiService.shutdownRaspberryPi()
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert('Shutting down...');
          this.logout();
        }
        else {
          alert('Error');
        }
      },
      error => this.errorMessage = <any>error
    );
  }

  update() {
    this.raspberryPiService.updateRaspberryPi()
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert('Raspberry Pi firmware updated, rebooting...');
          this.logout();
        }
        else {
          alert('Raspberry Pi firmware already up-to-date');
        }
      },
      error => this.errorMessage = <any>error
    );
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  killProcess(processId: number) {
    this.raspberryPiService.killProcess(processId)
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert(`Process #${processId} killed`);
          this.raspberryPi.cpu.loadStatus.processes =
            remove(this.raspberryPi.cpu.loadStatus.processes, (process) => {
              return process.processId !== processId;
            });
        }
        else {
          alert('Error');
        }
      },
      error => this.errorMessage = <any>error
    );
  }

  isAuthorizedToKill(processOwnerUsername: string) {
    if (this.isSuperUser) {
      return true;
    }
    const username = this.authService.getLoggedInUsername();
    if (endsWith(processOwnerUsername, '+')){
      return startsWith(username, trimEnd(processOwnerUsername, '+'));
    }
    return username === processOwnerUsername;
  }

  overclock(cpuMaxFrequencyLevel: CpuMaxFrequencyLevel) {
    this.raspberryPiService.overclockRaspberryPi(cpuMaxFrequencyLevel)
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert(`Raspberry Pi overclocked to ${CpuMaxFrequencyLevel[cpuMaxFrequencyLevel]} level, rebooting...`);
          this.logout();
        }
        else {
          alert(`Raspberry Pi already overclocked to ${CpuMaxFrequencyLevel[cpuMaxFrequencyLevel]} level`);
        }
      },
      error => this.errorMessage = <any>error
    );
  }

  getOrderedAndMappedCpuNormalizedFrequencies() {
    const maxFrequency = max(map(this.raspberryPi.cpu.frequencies, 'value'));
    const minFrequency = min(map(this.raspberryPi.cpu.frequencies, 'value'));
    const frequencyData = map(this.raspberryPi.cpu.frequencies, (frequency: ICpuFrequency) => {
      return {
        value: 100 * ((frequency.value - minFrequency) / (maxFrequency - minFrequency)),
        name: new Date(frequency.dateTime),
        absoluteValue: frequency.value
      };
    });
    return orderBy(frequencyData, 'name');
  }

  getOrderedAndMappedCpuTemperatures() {
    const temperatureData = map(this.raspberryPi.cpu.temperatures, (temperature: ICpuTemperature) => {
      return {
        value: temperature.value,
        name: new Date(temperature.dateTime)
      };
    });
    return orderBy(temperatureData, 'name');
  }

  getOrderedAndMappedCpuLoadStatuses() {
    const loadStatusData = map(this.raspberryPi.cpu.loadStatuses, (loadStatus: ICpuLoadStatus) => {
      return {
        value: loadStatus.totalRealTime,
        name: new Date(loadStatus.dateTime),
        processes: orderBy(
          loadStatus.processes,
          ['cpuPercentage', 'ramPercentage'],
          ['desc', 'desc'])
      };
    });
    return orderBy(loadStatusData, 'name');
  }

  getOrderedAndMappedRamStatuses() {
    const ramStatusData = map(this.raspberryPi.ram.statuses, (memoryStatus: IRandomAccessMemoryStatus) => {
      const total = memoryStatus.free + memoryStatus.used + memoryStatus.diskCache;
      return {
        value: total === 0 ? 0 : 100 * memoryStatus.used / total,
        name: new Date(memoryStatus.dateTime)
      };
    });
    return orderBy(ramStatusData, 'name');
  }

  getOrderedAndMappedSwapMemoryStatuses() {
    const swapMemoryStatusData = map(this.raspberryPi.swapMemory.statuses, (memoryStatus: IMemoryStatus) => {
      const total = memoryStatus.free + memoryStatus.used;
      return {
        value: total === 0 ? 0 : 100 * memoryStatus.used / total,
        name: new Date(memoryStatus.dateTime)
      };
    });
    return orderBy(swapMemoryStatusData, 'name');
  }

  getOrderedAndMappedRxNetworkInterfaceNormalizedStatuses(interfaceName: string) {
    const networkInterface = find(this.raspberryPi.network.networkInterfaces, { 'name': interfaceName });
    const maxReceiveSpeed = max(map(networkInterface.statuses, 'receiveSpeed'));
    const minReceiveSpeed = min(map(networkInterface.statuses, 'receiveSpeed'));
    const receiveSpeedData = map(networkInterface.statuses, (status: INetworkInterfaceStatus) => {
      return {
        value: 100 * ((status.receiveSpeed - minReceiveSpeed) / (maxReceiveSpeed - minReceiveSpeed)),
        name: new Date(status.dateTime),
        absoluteValue: status.receiveSpeed
      };
    });
    return orderBy(receiveSpeedData, 'name');
  }

  getOrderedAndMappedTxNetworkInterfaceNormalizedStatuses(interfaceName: string) {
    const networkInterface = find(this.raspberryPi.network.networkInterfaces, { 'name': interfaceName });
    const maxSendSpeed = max(map(networkInterface.statuses, 'sendSpeed'));
    const minSendSpeed = min(map(networkInterface.statuses, 'sendSpeed'));
    const sendSpeedData = map(networkInterface.statuses, (status: INetworkInterfaceStatus) => {
      return {
        value: 100 * ((status.sendSpeed - minSendSpeed) / (maxSendSpeed - minSendSpeed)),
        name: new Date(status.dateTime),
        absoluteValue: status.sendSpeed
      };
    });
    return orderBy(sendSpeedData, 'name');
  }

}
