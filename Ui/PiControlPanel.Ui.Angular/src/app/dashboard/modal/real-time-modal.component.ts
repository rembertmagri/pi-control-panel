import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { orderBy, map, isNil } from 'lodash';
import {
  ICpuTemperature,
  ICpuLoadStatus,
  IMemoryStatus, 
  IRandomAccessMemoryStatus} from '../../shared/interfaces/raspberry-pi';
import { BsModalRef } from 'ngx-bootstrap';
import { CpuTemperatureService } from 'src/app/shared/services/cpu-temperature.service';
import { CpuLoadStatusService } from 'src/app/shared/services/cpu-load-status.service';
import { RamStatusService } from 'src/app/shared/services/ram-status.service';
import { SwapMemoryStatusService } from 'src/app/shared/services/swap-memory-status.service';

@Component({
  templateUrl: './real-time-modal.component.html',
  styleUrls: ['./real-time-modal.component.css']
})
export class RealTimeModalComponent implements OnInit, OnDestroy {
  errorMessage: string;
  chartData: any[];

  temperatureDataReady: boolean;
  loadStatusDataReady: boolean;
  ramStatusDataReady: boolean;
  swapMemoryStatusDataReady: boolean;

  cpuTemperatureBehaviorSubjectSubscription: Subscription;
  cpuLoadStatusBehaviorSubjectSubscription: Subscription;
  ramStatusBehaviorSubjectSubscription: Subscription;
  swapMemoryStatusBehaviorSubjectSubscription: Subscription;

  constructor(public bsModalRef: BsModalRef,
    private cpuTemperatureService: CpuTemperatureService,
    private cpuLoadStatusService: CpuLoadStatusService,
    private ramStatusService: RamStatusService,
    private swapMemoryStatusService: SwapMemoryStatusService) { }

  ngOnInit() {
    this.chartData = [
      { name: "CPU Temperature (°C)", series: [] },
      { name: "CPU Real-Time Load (%)", series: [] },
      { name: "RAM Usage (%)", series: [] },
      { name: "Swap Memory Usage (%)", series: [] }
    ];

    this.temperatureDataReady = false;
    this.cpuTemperatureBehaviorSubjectSubscription = this.cpuTemperatureService.getLastCpuTemperatures()
      .subscribe(
      result => {
        var temperatureData = map(result.items, (temperature: ICpuTemperature) => {
          return {
            value: temperature.value,
            name: new Date(temperature.dateTime)
          };
        });
        this.chartData[0].series = orderBy(temperatureData, 'name');
        this.chartData = [...this.chartData];
        if(!this.temperatureDataReady) {
          this.cpuTemperatureService.subscribeToNewCpuTemperatures();
        }
        this.temperatureDataReady = true;
      },
      error => this.errorMessage = <any>error
    );

    this.loadStatusDataReady = false;
    this.cpuLoadStatusBehaviorSubjectSubscription = this.cpuLoadStatusService.getLastCpuLoadStatuses()
      .subscribe(
        result => {
          var loadStatusData = map(result.items, (loadStatus: ICpuLoadStatus) => {
            return {
              value: loadStatus.totalRealTime,
              name: new Date(loadStatus.dateTime)
            };
          });
          this.chartData[1].series = orderBy(loadStatusData, 'name');
          this.chartData = [...this.chartData];
          if(!this.loadStatusDataReady) {
            this.cpuLoadStatusService.subscribeToNewCpuLoadStatuses();
          }
          this.loadStatusDataReady = true;
        },
        error => this.errorMessage = <any>error
      );

    this.ramStatusDataReady = false;
    this.ramStatusBehaviorSubjectSubscription = this.ramStatusService.getLastMemoryStatuses()
      .subscribe(
        result => {
          var memoryStatusData = map(result.items, (memoryStatus: IRandomAccessMemoryStatus) => {
            return {
              value: 100 * memoryStatus.used / (memoryStatus.free + memoryStatus.used + memoryStatus.diskCache),
              name: new Date(memoryStatus.dateTime)
            };
          });
          this.chartData[2].series = orderBy(memoryStatusData, 'name');
          this.chartData = [...this.chartData];
          if(!this.ramStatusDataReady) {
            this.ramStatusService.subscribeToNewMemoryStatuses();
          }
          this.ramStatusDataReady = true;
        },
        error => this.errorMessage = <any>error
      );

    this.swapMemoryStatusDataReady = false;
    this.swapMemoryStatusBehaviorSubjectSubscription = this.swapMemoryStatusService.getLastMemoryStatuses()
      .subscribe(
        result => {
          var memoryStatusData = map(result.items, (memoryStatus: IMemoryStatus) => {
            return {
              value: 100 * memoryStatus.used / (memoryStatus.free + memoryStatus.used),
              name: new Date(memoryStatus.dateTime)
            };
          });
          this.chartData[3].series = orderBy(memoryStatusData, 'name');
          this.chartData = [...this.chartData];
          if(!this.swapMemoryStatusDataReady) {
            this.swapMemoryStatusService.subscribeToNewMemoryStatuses();
          }
          this.swapMemoryStatusDataReady = true;
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

  isChartDataReady() {
    return this.temperatureDataReady && this.loadStatusDataReady &&
      this.ramStatusDataReady && this.swapMemoryStatusDataReady;
  }

  loadNextPage() {
    this.cpuTemperatureService.getNextPage();
    this.cpuLoadStatusService.getNextPage();
    this.ramStatusService.getNextPage();
    this.swapMemoryStatusService.getNextPage();
  }

  loadPreviousPage() {
    this.cpuTemperatureService.getPreviousPage();
    this.cpuLoadStatusService.getPreviousPage();
    this.ramStatusService.getPreviousPage();
    this.swapMemoryStatusService.getPreviousPage();
  }

  closeModal() {
    this.bsModalRef.hide();
  }
  
}
