namespace PiControlPanel.Application.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class NetworkService : BaseService<Network>, INetworkService
    {
        private readonly Persistence.Network.INetworkInterfaceStatusService persistenceNetworkInterfaceStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence network service.</param>
        /// <param name="persistenceNetworkInterfaceStatusService">The infrastructure layer persistence network interface status service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkService(
            Persistence.Network.INetworkService persistenceService,
            Persistence.Network.INetworkInterfaceStatusService persistenceNetworkInterfaceStatusService,
            OnDemand.INetworkService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceNetworkInterfaceStatusService = persistenceNetworkInterfaceStatusService;
        }

        /// <inheritdoc/>
        public async Task<NetworkInterfaceStatus> GetLastNetworkInterfaceStatusAsync(string networkInterfaceName)
        {
            this.logger.Debug("Application layer -> NetworkService -> GetLastNetworkInterfaceStatusAsync");
            return await this.persistenceNetworkInterfaceStatusService.GetLastAsync(networkInterfaceName);
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<NetworkInterfaceStatus>> GetNetworkInterfaceStatusesAsync(string networkInterfaceName, PagingInput pagingInput)
        {
            this.logger.Debug("Application layer -> NetworkService -> GetNetworkInterfaceStatusesAsync");
            return await this.persistenceNetworkInterfaceStatusService.GetPageAsync(networkInterfaceName, pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName)
        {
            this.logger.Debug("Application layer -> NetworkService -> GetNetworkInterfaceStatusObservable");
            return ((OnDemand.INetworkService)this.onDemandService).GetNetworkInterfaceStatusObservable(networkInterfaceName);
        }

        /// <inheritdoc/>
        public async Task SaveNetworkInterfacesStatusAsync(int samplingInterval)
        {
            this.logger.Debug("Application layer -> NetworkService -> SaveNetworkInterfacesStatusAsync");

            var network = await this.persistenceService.GetAsync();
            if (network == null)
            {
                this.logger.Info("Network information not available yet, returning...");
                await Task.Delay(samplingInterval);
                return;
            }

            var networkInterfaceNames = network.NetworkInterfaces.Select(i => i.Name).ToList();
            var networkInterfacesStatus = await ((OnDemand.INetworkService)this.onDemandService).GetNetworkInterfacesStatusAsync(networkInterfaceNames, samplingInterval);

            await this.persistenceNetworkInterfaceStatusService.AddManyAsync(networkInterfacesStatus);
            ((OnDemand.INetworkService)this.onDemandService).PublishNetworkInterfacesStatus(networkInterfacesStatus);
        }
    }
}
