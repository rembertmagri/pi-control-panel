import { Component, OnInit } from '@angular/core';
import { CpuFrequencyService } from '@services/cpu-frequency.service';
import { CpuTemperatureService } from '@services/cpu-temperature.service';
import { CpuLoadStatusService } from '@services/cpu-load-status.service';
import { RamStatusService } from '@services/ram-status.service';
import { SwapMemoryStatusService } from '@services/swap-memory-status.service';
import { NetworkInterfaceStatusService } from '@services/network-interface-status.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { isNil, get } from 'lodash';

@Component({
  templateUrl: './real-time-modal.component.html',
  styleUrls: ['./real-time-modal.component.css']
})
export class RealTimeModalComponent implements OnInit {
  errorMessage: string;
  public chartData: any[];
  colorScheme = {
    domain: [
      '#C39BD3', // CPU Frequency
      '#EC7063', // CPU Temperature
      '#E59866', // CPU Real-Time Load
      '#5499C7', // RAM Usage
      '#85929E', // Swap Memory Usage
      // Network Interfaces
      '#3B874E', // Interface 1 Rx
      '#83873B', // Interface 1 Tx
      '#5CB773', // Interface 2 Rx
      '#B3B75C', // Interface 2 Tx
      '#95D0A4', // Interface 3 Rx
      '#CDD095', // Interface 3 Tx
      '#D1EBD7', // Interface 4 Rx
      '#E5E6C6'  // Interface 4 Tx
    ]
  };

  constructor(public bsModalRef: BsModalRef,
    private cpuFrequencyService: CpuFrequencyService,
    private cpuTemperatureService: CpuTemperatureService,
    private cpuLoadStatusService: CpuLoadStatusService,
    private ramStatusService: RamStatusService,
    private swapMemoryStatusService: SwapMemoryStatusService,
    private networkInterfaceStatusService: NetworkInterfaceStatusService) { }

  ngOnInit() {
    this.cpuFrequencyService.refetch();
    this.cpuTemperatureService.refetch();
    this.cpuLoadStatusService.refetch();
    this.ramStatusService.refetch();
    this.swapMemoryStatusService.refetch();
    this.networkInterfaceStatusService.refetch();
  }

  isChartDataReady() {
    return !isNil(this.chartData);
  }

  loadNextPage() {
    this.cpuFrequencyService.getNextPage();
    this.cpuTemperatureService.getNextPage();
    this.cpuLoadStatusService.getNextPage();
    this.ramStatusService.getNextPage();
    this.swapMemoryStatusService.getNextPage();
    this.networkInterfaceStatusService.getNextPage();
  }

  loadPreviousPage() {
    this.cpuFrequencyService.getPreviousPage();
    this.cpuTemperatureService.getPreviousPage();
    this.cpuLoadStatusService.getPreviousPage();
    this.ramStatusService.getPreviousPage();
    this.swapMemoryStatusService.getPreviousPage();
    this.networkInterfaceStatusService.getPreviousPage();
  }

  closeModal() {
    this.bsModalRef.hide();
  }

  getValue(model: any) {
    return get(model, 'absoluteValue', model.value);
  }
  
}
