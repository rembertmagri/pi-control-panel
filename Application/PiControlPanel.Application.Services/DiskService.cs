namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class DiskService : BaseService<Disk>, IDiskService
    {
        private readonly Persistence.Disk.IDiskStatusService persistenceStatusService;

        public DiskService(
            Persistence.Disk.IDiskService persistenceService,
            Persistence.Disk.IDiskStatusService persistenceStatusService,
            OnDemand.IDiskService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        public async Task<DiskStatus> GetLastStatusAsync()
        {
            logger.Info("Application layer -> DiskService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        public async Task<PagingOutput<DiskStatus>> GetStatusesAsync(PagingInput pagingInput)
        {
            logger.Info("Application layer -> DiskService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        public IObservable<DiskStatus> GetStatusObservable()
        {
            logger.Info("Application layer -> DiskService -> GetStatusObservable");
            return ((OnDemand.IDiskService)this.onDemandService).GetStatusObservable();
        }

        public async Task SaveStatusAsync()
        {
            logger.Info("Application layer -> DiskService -> SaveStatusAsync");
            var status = await ((OnDemand.IDiskService)this.onDemandService).GetStatusAsync();

            ((OnDemand.IDiskService)this.onDemandService).PublishStatus(status);
            await this.persistenceStatusService.AddAsync(status);
        }

        protected override async Task<Disk> GetPersistedInfoAsync(Disk onDemandInfo)
        {
            return await ((Persistence.Disk.IDiskService)this.persistenceService)
                .GetAsync(onDemandInfo.FileSystem);
        }
    }
}
