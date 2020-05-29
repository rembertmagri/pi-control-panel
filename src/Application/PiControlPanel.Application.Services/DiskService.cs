﻿namespace PiControlPanel.Application.Services
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
            this.logger.Debug("Application layer -> DiskService -> GetLastFileSystemStatusAsync");
            return await this.persistenceStatusService.GetLastAsync(fileSystemName);
        }

        public async Task<PagingOutput<FileSystemStatus>> GetFileSystemStatusesAsync(string fileSystemName, PagingInput pagingInput)
        {
            this.logger.Debug("Application layer -> DiskService -> GetFileSystemStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(fileSystemName, pagingInput);
        }

        public IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName)
        {
            this.logger.Debug("Application layer -> DiskService -> GetFileSystemStatusObservable");
            return ((OnDemand.IDiskService)this.onDemandService).GetFileSystemStatusObservable(fileSystemName);
        }

        public async Task SaveFileSystemStatusAsync()
        {
            this.logger.Debug("Application layer -> DiskService -> SaveFileSystemStatusAsync");

            var disk = await this.persistenceService.GetAsync();
            if (disk == null)
            {
                this.logger.Info("Disk information not available yet, returning...");
                return;
            }

            var fileSystemNames = disk.FileSystems.Select(i => i.Name).ToList();
            var fileSystemsStatus = await ((OnDemand.IDiskService)this.onDemandService).GetFileSystemsStatusAsync(fileSystemNames);

            await this.persistenceStatusService.AddManyAsync(fileSystemsStatus);
            ((OnDemand.IDiskService)this.onDemandService).PublishFileSystemsStatus(fileSystemsStatus);
        }
    }
}
