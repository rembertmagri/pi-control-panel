namespace PiControlPanel.Application.Services
{
    using System;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class MemoryService<TMemory, TMemoryStatus> : BaseService<TMemory>, IMemoryService<TMemory, TMemoryStatus>
        where TMemory : Memory
        where TMemoryStatus : MemoryStatus
    {
        private readonly Persistence.Memory.IMemoryStatusService<TMemoryStatus> persistenceStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryService{TMemory, TMemoryStatus}"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence memory service.</param>
        /// <param name="persistenceStatusService">The infrastructure layer persistence memory status service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public MemoryService(
            Persistence.Memory.IMemoryService<TMemory> persistenceService,
            Persistence.Memory.IMemoryStatusService<TMemoryStatus> persistenceStatusService,
            OnDemand.IMemoryService<TMemory, TMemoryStatus> onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        /// <inheritdoc/>
        public async Task<TMemoryStatus> GetLastStatusAsync()
        {
            this.logger.Debug("Application layer -> MemoryService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<TMemoryStatus>> GetStatusesAsync(PagingInput pagingInput)
        {
            this.logger.Debug("Application layer -> MemoryService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<TMemoryStatus> GetStatusObservable()
        {
            this.logger.Debug("Application layer -> MemoryService -> GetStatusObservable");
            return ((OnDemand.IMemoryService<TMemory, TMemoryStatus>)this.onDemandService).GetStatusObservable();
        }

        /// <inheritdoc/>
        public async Task SaveStatusAsync()
        {
            this.logger.Debug("Application layer -> MemoryService -> SaveStatusAsync");
            var status = await ((OnDemand.IMemoryService<TMemory, TMemoryStatus>)this.onDemandService).GetStatusAsync();

            await this.persistenceStatusService.AddAsync(status);
            ((OnDemand.IMemoryService<TMemory, TMemoryStatus>)this.onDemandService).PublishStatus(status);
        }
    }
}
