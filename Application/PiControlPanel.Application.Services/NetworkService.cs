namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class NetworkService : BaseService<Network>, INetworkService
    {
        private readonly Persistence.Network.INetworkInterfaceStatusService persistenceNetworkInterfaceStatusService;

        public NetworkService(
            Persistence.Network.INetworkService persistenceService,
            Persistence.Network.INetworkInterfaceStatusService persistenceNetworkInterfaceStatusService,
            OnDemand.INetworkService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceNetworkInterfaceStatusService = persistenceNetworkInterfaceStatusService;
        }

        public async Task<NetworkInterfaceStatus> GetLastNetworkInterfaceStatusAsync(string networkInterfaceName)
        {
            logger.Info("Application layer -> NetworkService -> GetLastNetworkInterfaceStatusAsync");
            return await this.persistenceNetworkInterfaceStatusService.GetLastAsync(networkInterfaceName);
        }

        public async Task<PagingOutput<NetworkInterfaceStatus>> GetNetworkInterfaceStatusesAsync(string networkInterfaceName, PagingInput pagingInput)
        {
            logger.Info("Application layer -> NetworkService -> GetNetworkInterfaceStatusesAsync");
            return await this.persistenceNetworkInterfaceStatusService.GetPageAsync(networkInterfaceName, pagingInput);
        }

        public IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName)
        {
            logger.Info("Application layer -> NetworkService -> GetNetworkInterfaceStatusObservable");
            return ((OnDemand.INetworkService)this.onDemandService).GetNetworkInterfaceStatusObservable(networkInterfaceName);
        }

        public async Task SaveNetworkInterfacesStatusAsync(int samplingInterval)
        {
            logger.Info("Application layer -> NetworkService -> SaveNetworkInterfacesStatusAsync");

            var network = await this.persistenceService.GetAsync();
            if (network == null)
            {
                logger.Info("Network information not available yet, returning...");
                await Task.Delay(samplingInterval);
                return;
            }

            var networkInterfaceNames = network.NetworkInterfaces.Select(i => i.Name).ToList();
            var networkInterfacesStatus = await ((OnDemand.INetworkService)this.onDemandService).GetNetworkInterfacesStatusAsync(networkInterfaceNames, samplingInterval);

            foreach (var networkInterfaceStatus in networkInterfacesStatus)
            {
                await this.persistenceNetworkInterfaceStatusService.AddAsync(networkInterfaceStatus);
            }
            ((OnDemand.INetworkService)this.onDemandService).PublishNetworkInterfacesStatus(networkInterfacesStatus);
        }
    }
}
