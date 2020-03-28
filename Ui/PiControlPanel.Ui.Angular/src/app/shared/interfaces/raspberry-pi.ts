export interface IRaspberryPi {
  chipset: IChipset;
  cpu: ICpu;
  disk: IDisk;
  memory: IMemory;
  gpu: IGpu;
  os: IOs;
}

export interface IChipset {
  model: string;
  revision: string;
  serial: string;
  version: string;
}

export interface ICpu {
  cores: string;
  model: string;
  temperature: ICpuTemperature;
  averageLoad: ICpuAverageLoad;
  realTimeLoad: ICpuRealTimeLoad;
}

export interface ICpuTemperature {
  value: number;
  dateTime: string;
}

export interface ICpuAverageLoad {
  lastMinute: number;
  last5Minutes: number;
  last15Minutes: number;
  dateTime: string;
}

export interface ICpuRealTimeLoad {
  kernel: number;
  user: number;
  total: number;
  dateTime: string;
}

export interface IDisk {
  fileSystem: string;
  type: string;
  total: number;
  status: IDiskStatus;
}

export interface IDiskStatus {
  used: number;
  available: number;
  dateTime: string;
}

export interface IMemory {
  total: number;
  status: IMemoryStatus;
}

export interface IMemoryStatus {
  used: number;
  free: number;
  dateTime: string;
}

export interface IGpu {
  memory: number;
}

export interface IOs {
  name: string;
  kernel: string;
  hostname: string;
}
