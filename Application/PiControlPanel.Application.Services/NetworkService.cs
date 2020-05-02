namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class NetworkService : BaseService<Network>, INetworkService
    {
        public NetworkService(
            Persistence.Network.INetworkService persistenceService,
            OnDemand.INetworkService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
        }
    }
}
