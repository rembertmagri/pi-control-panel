namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;

    /// <inheritdoc/>
    public class NetworkWorker : BaseWorker<Network>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkWorker"/> class.
        /// </summary>
        /// <param name="networkService">The application layer NetworkService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkWorker(
            INetworkService networkService,
            IConfiguration configuration,
            ILogger logger)
            : base(networkService, configuration, logger)
        {
        }
    }
}
