﻿namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class ChipsetService : BaseService<Chipset>, IChipsetService
    {
        public ChipsetService(
            Persistence.IChipsetService persistenceService,
            OnDemand.IChipsetService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
        }

        protected override async Task<Chipset> GetPersistedInfoAsync(Chipset onDemandInfo)
        {
            return await ((Persistence.IChipsetService)this.persistenceService)
                .GetAsync(onDemandInfo.Serial);
        }
    }
}
