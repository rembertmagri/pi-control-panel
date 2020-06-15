namespace PiControlPanel.Infrastructure.Persistence.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControlPanelDbContext databaseContext;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// This constructor signature exists for integration tests purposes only.
        /// </summary>
        /// <param name="databaseContext">The ControlPanelDbContext instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public UnitOfWork(ControlPanelDbContext databaseContext, ILogger logger)
        {
            this.databaseContext = databaseContext;
            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public UnitOfWork(IConfiguration configuration, ILogger logger)
        {
            this.databaseContext = new ControlPanelDbContext(configuration);
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Chipset> ChipsetRepository => new RepositoryBase<Entities.Chipset>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Cpu.Cpu> CpuRepository => new RepositoryBase<Entities.Cpu.Cpu>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Cpu.CpuFrequency> CpuFrequencyRepository => new RepositoryBase<Entities.Cpu.CpuFrequency>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Cpu.CpuSensorsStatus> CpuSensorsStatusRepository => new RepositoryBase<Entities.Cpu.CpuSensorsStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Cpu.CpuLoadStatus> CpuLoadStatusRepository => new RepositoryBase<Entities.Cpu.CpuLoadStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Gpu> GpuRepository => new RepositoryBase<Entities.Gpu>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Os.Os> OsRepository => new RepositoryBase<Entities.Os.Os>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Os.OsStatus> OsStatusRepository => new RepositoryBase<Entities.Os.OsStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Disk.Disk> DiskRepository => new RepositoryBase<Entities.Disk.Disk>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Disk.FileSystem> FileSystemRepository => new RepositoryBase<Entities.Disk.FileSystem>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Disk.FileSystemStatus> FileSystemStatusRepository => new RepositoryBase<Entities.Disk.FileSystemStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Memory.RandomAccessMemory> RandomAccessMemoryRepository => new RepositoryBase<Entities.Memory.RandomAccessMemory>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatusRepository => new RepositoryBase<Entities.Memory.RandomAccessMemoryStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Memory.SwapMemory> SwapMemoryRepository => new RepositoryBase<Entities.Memory.SwapMemory>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Memory.SwapMemoryStatus> SwapMemoryStatusRepository => new RepositoryBase<Entities.Memory.SwapMemoryStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Network.Network> NetworkRepository => new RepositoryBase<Entities.Network.Network>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Network.NetworkInterface> NetworkInterfaceRepository => new RepositoryBase<Entities.Network.NetworkInterface>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public IRepositoryBase<Entities.Network.NetworkInterfaceStatus> NetworkInterfaceStatusRepository => new RepositoryBase<Entities.Network.NetworkInterfaceStatus>(this.databaseContext, this.logger);

        /// <inheritdoc/>
        public void Commit()
        {
            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task CommitAsync()
        {
            await this.databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.databaseContext.Dispose();
        }

        /// <inheritdoc/>
        public void RejectChanges()
        {
            var changedEntries = this.databaseContext.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList();
            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
