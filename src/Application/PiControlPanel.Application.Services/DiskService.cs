namespace PiControlPanel.Application.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class DiskService : BaseService<Disk>, IDiskService
    {
        private readonly Persistence.Disk.IFileSystemStatusService persistenceStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence disk service.</param>
        /// <param name="persistenceStatusService">The infrastructure layer persistence disk status service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskService(
            Persistence.Disk.IDiskService persistenceService,
            Persistence.Disk.IFileSystemStatusService persistenceStatusService,
            OnDemand.IDiskService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        /// <inheritdoc/>
        public async Task<FileSystemStatus> GetLastFileSystemStatusAsync(string fileSystemName)
        {
            this.Logger.Debug("Application layer -> DiskService -> GetLastFileSystemStatusAsync");
            return await this.persistenceStatusService.GetLastAsync(fileSystemName);
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<FileSystemStatus>> GetFileSystemStatusesAsync(string fileSystemName, PagingInput pagingInput)
        {
            this.Logger.Debug("Application layer -> DiskService -> GetFileSystemStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(fileSystemName, pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName)
        {
            this.Logger.Debug("Application layer -> DiskService -> GetFileSystemStatusObservable");
            return ((OnDemand.IDiskService)this.OnDemandService).GetFileSystemStatusObservable(fileSystemName);
        }

        /// <inheritdoc/>
        public async Task SaveFileSystemStatusAsync()
        {
            this.Logger.Debug("Application layer -> DiskService -> SaveFileSystemStatusAsync");

            var disk = await this.PersistenceService.GetAsync();
            if (disk == null)
            {
                this.Logger.Info("Disk information not available yet, returning...");
                return;
            }

            var fileSystemNames = disk.FileSystems.Select(i => i.Name).ToList();
            var fileSystemsStatus = await ((OnDemand.IDiskService)this.OnDemandService).GetFileSystemsStatusAsync(fileSystemNames);

            await this.persistenceStatusService.AddManyAsync(fileSystemsStatus);
            ((OnDemand.IDiskService)this.OnDemandService).PublishFileSystemsStatus(fileSystemsStatus);
        }
    }
}
