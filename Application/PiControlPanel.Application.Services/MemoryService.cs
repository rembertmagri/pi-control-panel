namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using System;

    public class MemoryService : BaseService<Memory>, IMemoryService
    {
        private readonly Persistence.Memory.IMemoryStatusService persistenceStatusService;

        public MemoryService(
            Persistence.Memory.IMemoryService persistenceService,
            Persistence.Memory.IMemoryStatusService persistenceStatusService,
            OnDemand.IMemoryService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        public async Task<MemoryStatus> GetLastStatusAsync()
        {
            logger.Info("Application layer -> MemoryService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        public async Task<PagingOutput<MemoryStatus>> GetStatusesAsync(PagingInput pagingInput)
        {
            logger.Info("Application layer -> MemoryService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        public IObservable<MemoryStatus> GetStatusObservable()
        {
            logger.Info("Application layer -> MemoryService -> GetStatusObservable");
            return ((OnDemand.IMemoryService)this.onDemandService).GetStatusObservable();
        }

        public async Task SaveStatusAsync()
        {
            logger.Info("Application layer -> MemoryService -> SaveStatusAsync");
            var status = await ((OnDemand.IMemoryService)this.onDemandService).GetStatusAsync();

            await this.persistenceStatusService.AddAsync(status);
            ((OnDemand.IMemoryService)this.onDemandService).PublishStatus(status);
        }
    }
}
