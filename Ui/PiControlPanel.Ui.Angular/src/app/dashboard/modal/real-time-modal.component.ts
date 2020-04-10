import { Component, OnInit } from '@angular/core';
import { CpuFrequencyService } from 'src/app/shared/services/cpu-frequency.service';
import { CpuTemperatureService } from 'src/app/shared/services/cpu-temperature.service';
import { CpuLoadStatusService } from 'src/app/shared/services/cpu-load-status.service';
import { RamStatusService } from 'src/app/shared/services/ram-status.service';
import { SwapMemoryStatusService } from 'src/app/shared/services/swap-memory-status.service';
import { BsModalRef } from 'ngx-bootstrap';
import { isNil, get } from 'lodash';

@Component({
  templateUrl: './real-time-modal.component.html',
  styleUrls: ['./real-time-modal.component.css']
})
export class RealTimeModalComponent implements OnInit {
  errorMessage: string;
  public chartData: any[];
  colorScheme = {
    domain: ['#C39BD3', '#EC7063', '#E59866', '#5499C7', '#85929E']
  };

  constructor(public bsModalRef: BsModalRef,
    private cpuFrequencyService: CpuFrequencyService,
    private cpuTemperatureService: CpuTemperatureService,
    private cpuLoadStatusService: CpuLoadStatusService,
    private ramStatusService: RamStatusService,
    private swapMemoryStatusService: SwapMemoryStatusService) { }

  ngOnInit() {
    
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
  }

  loadPreviousPage() {
    this.cpuFrequencyService.getPreviousPage();
    this.cpuTemperatureService.getPreviousPage();
    this.cpuLoadStatusService.getPreviousPage();
    this.ramStatusService.getPreviousPage();
    this.swapMemoryStatusService.getPreviousPage();
  }

  closeModal() {
    this.bsModalRef.hide();
  }

  getValue(model: any) {
    return get(model, 'absoluteValue', model.value);
  }
  
}
