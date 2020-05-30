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

    /// <inheritdoc/>
    public class OsService : BaseService<Os>, IOsService
    {
        private readonly Persistence.Os.IOsStatusService persistenceStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence operating system service.</param>
        /// <param name="persistenceStatusService">The infrastructure layer persistence operating system status service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsService(
            Persistence.Os.IOsService persistenceService,
            Persistence.Os.IOsStatusService persistenceStatusService,
            OnDemand.IOsService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceStatusService = persistenceStatusService;
        }

        /// <inheritdoc/>
        public async Task<OsStatus> GetLastStatusAsync()
        {
            this.logger.Debug("Application layer -> OsService -> GetLastStatusAsync");
            return await this.persistenceStatusService.GetLastAsync();
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<OsStatus>> GetStatusesAsync(PagingInput pagingInput)
        {
            this.logger.Debug("Application layer -> OsService -> GetStatusesAsync");
            return await this.persistenceStatusService.GetPageAsync(pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<OsStatus> GetStatusObservable()
        {
            this.logger.Debug("Application layer -> OsService -> GetStatusObservable");
            return ((OnDemand.IOsService)this.onDemandService).GetStatusObservable();
        }

        /// <inheritdoc/>
        public async Task SaveStatusAsync()
        {
            this.logger.Debug("Application layer -> OsService -> SaveStatusAsync");
            var status = await ((OnDemand.IOsService)this.onDemandService).GetStatusAsync();

            await this.persistenceStatusService.AddAsync(status);
            ((OnDemand.IOsService)this.onDemandService).PublishStatus(status);
        }
    }
}
