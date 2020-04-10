namespace PiControlPanel.Infrastructure.Persistence.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using System.Linq;
    using System.Threading.Tasks;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControlPanelDbContext dbContext;
        private readonly ILogger logger;

        public UnitOfWork(IConfiguration configuration, ILogger logger)
        {
            this.dbContext = new ControlPanelDbContext(configuration);
            this.logger = logger;
        }

        public void Commit()
        {
            this.dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await this.dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        public void RejectChanges()
        {
            var changedEntries = dbContext.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList();
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

        #region Repositories

        public IRepositoryBase<Entities.Chipset> ChipsetRepository => new RepositoryBase<Entities.Chipset>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Cpu.Cpu> CpuRepository => new RepositoryBase<Entities.Cpu.Cpu>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Cpu.CpuFrequency> CpuFrequencyRepository => new RepositoryBase<Entities.Cpu.CpuFrequency>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Cpu.CpuTemperature> CpuTemperatureRepository => new RepositoryBase<Entities.Cpu.CpuTemperature>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Cpu.CpuLoadStatus> CpuLoadStatusRepository => new RepositoryBase<Entities.Cpu.CpuLoadStatus>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Gpu> GpuRepository => new RepositoryBase<Entities.Gpu>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Os.Os> OsRepository => new RepositoryBase<Entities.Os.Os>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Os.OsStatus> OsStatusRepository => new RepositoryBase<Entities.Os.OsStatus>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Disk.Disk> DiskRepository => new RepositoryBase<Entities.Disk.Disk>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Disk.DiskStatus> DiskStatusRepository => new RepositoryBase<Entities.Disk.DiskStatus>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Memory.RandomAccessMemory> RandomAccessMemoryRepository => new RepositoryBase<Entities.Memory.RandomAccessMemory>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatusRepository => new RepositoryBase<Entities.Memory.RandomAccessMemoryStatus>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Memory.SwapMemory> SwapMemoryRepository => new RepositoryBase<Entities.Memory.SwapMemory>(this.dbContext, this.logger);

        public IRepositoryBase<Entities.Memory.SwapMemoryStatus> SwapMemoryStatusRepository => new RepositoryBase<Entities.Memory.SwapMemoryStatus>(this.dbContext, this.logger);

        #endregion
    }
}
