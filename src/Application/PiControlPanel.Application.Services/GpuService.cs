namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class GpuService : BaseService<Gpu>, IGpuService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpuService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public GpuService(
            Persistence.IGpuService persistenceService,
            OnDemand.IGpuService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
        }
    }
}
