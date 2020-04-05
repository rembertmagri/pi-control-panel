export interface IRaspberryPi {
  chipset: IChipset;
  cpu: ICpu;
  disk: IDisk;
  ram: IRandomAccessMemory;
  swapMemory: IMemory;
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
  temperatures: ICpuTemperature[];
  loadStatus: ICpuLoadStatus;
  loadStatuses: ICpuLoadStatus[];
}

export interface ICpuTemperature {
  value: number;
  dateTime: string;
}

export interface ICpuLoadStatus {
  dateTime: string;
  lastMinuteAverage: number;
  last5MinutesAverage: number;
  last15MinutesAverage: number;
  kernelRealTime: number;
  userRealTime: number;
  totalRealTime: number;
  processes: ICpuProcess[];
}

export interface ICpuProcess {
  processId: number;
  user: string;
  priority: string;
  niceValue: number;
  totalMemory: number;
  ram: number;
  sharedMemory: number;
  state: string;
  cpuPercentage: number;
  ramPercentage: number;
  totalCpuTime: string;
  command: string;
}

export interface IDisk {
  fileSystem: string;
  type: string;
  total: number;
  status: IDiskStatus;
  statuses: IDiskStatus[];
}

export interface IDiskStatus {
  used: number;
  available: number;
  dateTime: string;
}

export interface IMemory {
  total: number;
  status: IMemoryStatus;
  statuses: IMemoryStatus[];
}

export interface IMemoryStatus {
  used: number;
  free: number;
  dateTime: string;
}

export interface IRandomAccessMemory extends IMemory {
  status: IRandomAccessMemoryStatus;
  statuses: IRandomAccessMemoryStatus[];
}

export interface IRandomAccessMemoryStatus extends IMemoryStatus {
  diskCache: number;
}

export interface IGpu {
  memory: number;
}

export interface IOs {
  name: string;
  kernel: string;
  hostname: string;
  status: IOsStatus;
  statuses: IOsStatus[];
}

export interface IOsStatus {
  uptime: string;
  dateTime: string;
}