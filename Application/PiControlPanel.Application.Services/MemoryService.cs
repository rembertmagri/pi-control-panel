﻿namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using System;

    public class MemoryService<T, U> : BaseService<T>, IMemoryService<T, U>
        where T : Memory
        where U : MemoryStatus
    {
        private readonly Persistence.Memory.IMemoryStatusService<U> persistenceStatusService;

        public MemoryService(
            Persistence.Memory.IMemoryService<T> persistenceService,
            Persistence.Memory.IMemoryStatusService<U> persistenceStatusService,
            OnDemand.IMemoryService<T, U> onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        public async Task<U> GetLastStatusAsync()
        {
            logger.Info("Application layer -> MemoryService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        public async Task<PagingOutput<U>> GetStatusesAsync(PagingInput pagingInput)
        {
            logger.Info("Application layer -> MemoryService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        public IObservable<U> GetStatusObservable()
        {
            logger.Info("Application layer -> MemoryService -> GetStatusObservable");
            return ((OnDemand.IMemoryService<T, U>)this.onDemandService).GetStatusObservable();
        }

        public async Task SaveStatusAsync()
        {
            logger.Info("Application layer -> MemoryService -> SaveStatusAsync");
            var status = await ((OnDemand.IMemoryService<T, U>)this.onDemandService).GetStatusAsync();

            await this.persistenceStatusService.AddAsync(status);
            ((OnDemand.IMemoryService<T, U>)this.onDemandService).PublishStatus(status);
        }
    }
}
