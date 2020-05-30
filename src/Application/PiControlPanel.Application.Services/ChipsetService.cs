namespace PiControlPanel.Application.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class ChipsetService : BaseService<Chipset>, IChipsetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipsetService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ChipsetService(
            Persistence.IChipsetService persistenceService,
            OnDemand.IChipsetService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
        }

        /// <inheritdoc/>
        protected override async Task<Chipset> GetPersistedInfoAsync(Chipset onDemandInfo)
        {
            return await ((Persistence.IChipsetService)this.PersistenceService)
                .GetAsync(onDemandInfo.Serial);
        }
    }
}
