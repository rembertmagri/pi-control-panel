namespace PiControlPanel.Application.Services
{
    using System;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Paging;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class OsService : BaseService<Os>, IOsService
    {
        private readonly Persistence.Os.IOsStatusService persistenceStatusService;

        public OsService(
            Persistence.Os.IOsService persistenceService,
            Persistence.Os.IOsStatusService persistenceStatusService,
            OnDemand.IOsService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        public async Task<OsStatus> GetLastStatusAsync()
        {
            this.logger.Debug("Application layer -> OsService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        public async Task<PagingOutput<OsStatus>> GetStatusesAsync(PagingInput pagingInput)
        {
            this.logger.Debug("Application layer -> OsService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        public IObservable<OsStatus> GetStatusObservable()
        {
            this.logger.Debug("Application layer -> OsService -> GetStatusObservable");
            return ((OnDemand.IOsService)this.onDemandService).GetStatusObservable();
        }

        public async Task SaveStatusAsync()
        {
            this.logger.Debug("Application layer -> OsService -> SaveStatusAsync");
            var status = await ((OnDemand.IOsService)this.onDemandService).GetStatusAsync();

            await this.persistenceStatusService.AddAsync(status);
            ((OnDemand.IOsService)this.onDemandService).PublishStatus(status);
        }
    }
}
