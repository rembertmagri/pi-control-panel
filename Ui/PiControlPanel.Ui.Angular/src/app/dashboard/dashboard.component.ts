import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RaspberryPiService } from '../shared/services/raspberry-pi.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { 
  IRaspberryPi,
  ICpuTemperature,
  ICpuLoadStatus,
  IMemoryStatus, 
  IRandomAccessMemoryStatus } from '../shared/interfaces/raspberry-pi';
import { AuthService } from '../shared/services/auth.service';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { remove, orderBy, map, isNil } from 'lodash';
import { RealTimeModalComponent } from './modal/real-time-modal.component';
import { CpuTemperatureService } from 'src/app/shared/services/cpu-temperature.service';
import { CpuLoadStatusService } from 'src/app/shared/services/cpu-load-status.service';
import { RamStatusService } from 'src/app/shared/services/ram-status.service';
import { SwapMemoryStatusService } from 'src/app/shared/services/swap-memory-status.service';

@Component({
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  raspberryPi: IRaspberryPi;
  errorMessage: string;
  modalRef: BsModalRef;

  subscribedToNewCpuTemperatures: boolean;
  subscribedToNewCpuLoadStatuses: boolean;
  subscribedToNewRamStatuses: boolean;
  subscribedToNewSwapMemoryStatuses: boolean;

  cpuTemperatureBehaviorSubjectSubscription: Subscription;
  cpuLoadStatusBehaviorSubjectSubscription: Subscription;
  ramStatusBehaviorSubjectSubscription: Subscription;
  swapMemoryStatusBehaviorSubjectSubscription: Subscription;

  constructor(private _route: ActivatedRoute,
    private router: Router,
    private modalService: BsModalService,
    private raspberryPiService: RaspberryPiService,
    private authService: AuthService,
    private cpuTemperatureService: CpuTemperatureService,
    private cpuLoadStatusService: CpuLoadStatusService,
    private ramStatusService: RamStatusService,
    private swapMemoryStatusService: SwapMemoryStatusService) { }

  ngOnInit() {
    this.raspberryPi = this._route.snapshot.data['raspberryPi'];

    this.subscribedToNewCpuTemperatures = false;
    this.cpuTemperatureBehaviorSubjectSubscription = this.cpuTemperatureService.getLastCpuTemperatures()
      .subscribe(
      result => {
        this.raspberryPi.cpu.temperatures = result.items;
        if(!isNil(this.modalRef)) {
          this.modalRef.content.chartData[0].series = this.getOrderedAndMappedCpuTemperatures();
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
          this.raspberryPi.cpu.loadStatuses = result.items;
          if(!isNil(this.modalRef)) {
            this.modalRef.content.chartData[1].series = this.getOrderedAndMappedCpuLoadStatuses();
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
          this.raspberryPi.ram.statuses = result.items;
          if(!isNil(this.modalRef)) {
            this.modalRef.content.chartData[2].series = this.getOrderedAndMappedRamStatuses();
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
          this.raspberryPi.swapMemory.statuses = result.items;
          if(!isNil(this.modalRef)) {
            this.modalRef.content.chartData[3].series = this.getOrderedAndMappedSwapMemoryStatuses();
            this.modalRef.content.chartData = [...this.modalRef.content.chartData];
          }
          if(!this.subscribedToNewSwapMemoryStatuses) {
            this.swapMemoryStatusService.subscribeToNewMemoryStatuses();
            this.subscribedToNewSwapMemoryStatuses = true;
          }
        },
        error => this.errorMessage = <any>error
      );
  }

  ngOnDestroy(): void {
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
  }

  openModal() {
    this.modalRef = this.modalService.show(
      RealTimeModalComponent,
      {
        class: 'modal-xl'
      });
    this.modalRef.content.chartData = [
      { name: "CPU Temperature (°C)", series: this.getOrderedAndMappedCpuTemperatures() },
      { name: "CPU Real-Time Load (%)", series: this.getOrderedAndMappedCpuLoadStatuses() },
      { name: "RAM Usage (%)", series: this.getOrderedAndMappedRamStatuses() },
      { name: "Swap Memory Usage (%)", series: this.getOrderedAndMappedSwapMemoryStatuses() }
    ];
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

  getOrderedAndMappedCpuTemperatures() {
    var temperatureData = map(this.raspberryPi.cpu.temperatures, (temperature: ICpuTemperature) => {
      return {
        value: temperature.value,
        name: new Date(temperature.dateTime)
      };
    });
    return orderBy(temperatureData, 'name');
  }

  getOrderedAndMappedCpuLoadStatuses() {
    var loadStatusData = map(this.raspberryPi.cpu.loadStatuses, (loadStatus: ICpuLoadStatus) => {
      return {
        value: loadStatus.totalRealTime,
        name: new Date(loadStatus.dateTime)
      };
    });
    return orderBy(loadStatusData, 'name');
  }

  getOrderedAndMappedRamStatuses() {
    var ramStatusData = map(this.raspberryPi.ram.statuses, (memoryStatus: IRandomAccessMemoryStatus) => {
      return {
        value: 100 * memoryStatus.used / (memoryStatus.free + memoryStatus.used + memoryStatus.diskCache),
        name: new Date(memoryStatus.dateTime)
      };
    });
    return orderBy(ramStatusData, 'name');
  }

  getOrderedAndMappedSwapMemoryStatuses() {
    var swapMemoryStatusData = map(this.raspberryPi.swapMemory.statuses, (memoryStatus: IMemoryStatus) => {
      return {
        value: 100 * memoryStatus.used / (memoryStatus.free + memoryStatus.used),
        name: new Date(memoryStatus.dateTime)
      };
    });
    return orderBy(swapMemoryStatusData, 'name');
  }

}
