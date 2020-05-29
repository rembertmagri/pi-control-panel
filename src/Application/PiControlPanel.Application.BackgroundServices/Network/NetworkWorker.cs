namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;

    /// <inheritdoc/>
    public class NetworkWorker : BaseWorker<Network>
    {
        public NetworkWorker(
            INetworkService networkService,
            IConfiguration configuration,
            ILogger logger)
            : base(networkService, configuration, logger)
        {
        }
    }
}
