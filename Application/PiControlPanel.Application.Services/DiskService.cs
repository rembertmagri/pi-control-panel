namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class DiskService : BaseService<Disk>, IDiskService
    {
        private readonly Persistence.Disk.IFileSystemStatusService persistenceStatusService;

        public DiskService(
            Persistence.Disk.IDiskService persistenceService,
            Persistence.Disk.IFileSystemStatusService persistenceStatusService,
            OnDemand.IDiskService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        public async Task<FileSystemStatus> GetLastFileSystemStatusAsync(string fileSystemName)
        {
            logger.Trace("Application layer -> DiskService -> GetLastFileSystemStatusAsync");
            return await this.persistenceStatusService.GetLastAsync(fileSystemName);
        }

        public async Task<PagingOutput<FileSystemStatus>> GetFileSystemStatusesAsync(string fileSystemName, PagingInput pagingInput)
        {
            logger.Trace("Application layer -> DiskService -> GetFileSystemStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(fileSystemName, pagingInput);
        }

        public IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName)
        {
            logger.Trace("Application layer -> DiskService -> GetFileSystemStatusObservable");
            return ((OnDemand.IDiskService)this.onDemandService).GetFileSystemStatusObservable(fileSystemName);
        }

        public async Task SaveFileSystemStatusAsync()
        {
            logger.Trace("Application layer -> DiskService -> SaveFileSystemStatusAsync");

            var disk = await this.persistenceService.GetAsync();
            if (disk == null)
            {
                logger.Info("Disk information not available yet, returning...");
                return;
            }

            var fileSystemNames = disk.FileSystems.Select(i => i.Name).ToList();
            var fileSystemsStatus = await ((OnDemand.IDiskService)this.onDemandService).GetFileSystemsStatusAsync(fileSystemNames);

            foreach (var fileSystemStatus in fileSystemsStatus)
            {
                await this.persistenceStatusService.AddAsync(fileSystemStatus);
            }
            ((OnDemand.IDiskService)this.onDemandService).PublishFileSystemsStatus(fileSystemsStatus);
        }
    }
}
