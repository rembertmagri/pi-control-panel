namespace PiControlPanel.Infrastructure.Persistence.Contracts.Repositories
{
    /// <summary>
    /// Interface defining all repositories.
    /// </summary>
    public interface IRepositoryContainer
    {
        /// <summary>
        /// Gets the repository for the Chipset entity.
        /// </summary>
        IRepositoryBase<Entities.Chipset> ChipsetRepository { get; }

        /// <summary>
        /// Gets the repository for the Cpu entity.
        /// </summary>
        IRepositoryBase<Entities.Cpu.Cpu> CpuRepository { get; }

        /// <summary>
        /// Gets the repository for the CpuFrequency entity.
        /// </summary>
        IRepositoryBase<Entities.Cpu.CpuFrequency> CpuFrequencyRepository { get; }

        /// <summary>
        /// Gets the repository for the CpuTemperature entity.
        /// </summary>
        IRepositoryBase<Entities.Cpu.CpuTemperature> CpuTemperatureRepository { get; }

        /// <summary>
        /// Gets the repository for the CpuLoadStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Cpu.CpuLoadStatus> CpuLoadStatusRepository { get; }

        /// <summary>
        /// Gets the repository for the Gpu entity.
        /// </summary>
        IRepositoryBase<Entities.Gpu> GpuRepository { get; }

        /// <summary>
        /// Gets the repository for the Os entity.
        /// </summary>
        IRepositoryBase<Entities.Os.Os> OsRepository { get; }

        /// <summary>
        /// Gets the repository for the OsStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Os.OsStatus> OsStatusRepository { get; }

        /// <summary>
        /// Gets the repository for the Disk entity.
        /// </summary>
        IRepositoryBase<Entities.Disk.Disk> DiskRepository { get; }

        /// <summary>
        /// Gets the repository for the FileSystemStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Disk.FileSystemStatus> FileSystemStatusRepository { get; }

        /// <summary>
        /// Gets the repository for the RandomAccessMemory entity.
        /// </summary>
        IRepositoryBase<Entities.Memory.RandomAccessMemory> RandomAccessMemoryRepository { get; }

        /// <summary>
        /// Gets the repository for the RandomAccessMemoryStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatusRepository { get; }

        /// <summary>
        /// Gets the repository for the SwapMemory entity.
        /// </summary>
        IRepositoryBase<Entities.Memory.SwapMemory> SwapMemoryRepository { get; }

        /// <summary>
        /// Gets the repository for the SwapMemoryStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Memory.SwapMemoryStatus> SwapMemoryStatusRepository { get; }

        /// <summary>
        /// Gets the repository for the Network entity.
        /// </summary>
        IRepositoryBase<Entities.Network.Network> NetworkRepository { get; }

        /// <summary>
        /// Gets the repository for the NetworkInterfaceStatus entity.
        /// </summary>
        IRepositoryBase<Entities.Network.NetworkInterfaceStatus> NetworkInterfaceStatusRepository { get; }
    }
}
