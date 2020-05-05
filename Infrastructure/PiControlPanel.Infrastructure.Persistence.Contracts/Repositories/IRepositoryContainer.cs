namespace PiControlPanel.Infrastructure.Persistence.Contracts.Repositories
{
    public interface IRepositoryContainer
    {
        IRepositoryBase<Entities.Chipset> ChipsetRepository { get; }

        IRepositoryBase<Entities.Cpu.Cpu> CpuRepository { get; }

        IRepositoryBase<Entities.Cpu.CpuFrequency> CpuFrequencyRepository { get; }

        IRepositoryBase<Entities.Cpu.CpuTemperature> CpuTemperatureRepository { get; }

        IRepositoryBase<Entities.Cpu.CpuLoadStatus> CpuLoadStatusRepository { get; }

        IRepositoryBase<Entities.Gpu> GpuRepository { get; }

        IRepositoryBase<Entities.Os.Os> OsRepository { get; }

        IRepositoryBase<Entities.Os.OsStatus> OsStatusRepository { get; }

        IRepositoryBase<Entities.Disk.Disk> DiskRepository { get; }

        IRepositoryBase<Entities.Disk.FileSystemStatus> FileSystemStatusRepository { get; }

        IRepositoryBase<Entities.Memory.RandomAccessMemory> RandomAccessMemoryRepository { get; }

        IRepositoryBase<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatusRepository { get; }

        IRepositoryBase<Entities.Memory.SwapMemory> SwapMemoryRepository { get; }

        IRepositoryBase<Entities.Memory.SwapMemoryStatus> SwapMemoryStatusRepository { get; }

        IRepositoryBase<Entities.Network.Network> NetworkRepository { get; }

        IRepositoryBase<Entities.Network.NetworkInterfaceStatus> NetworkInterfaceStatusRepository { get; }
    }
}
