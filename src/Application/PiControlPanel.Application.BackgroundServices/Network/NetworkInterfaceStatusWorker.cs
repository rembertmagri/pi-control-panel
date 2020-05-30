namespace PiControlPanel.Application.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    /// <inheritdoc/>
    public class NetworkInterfaceStatusWorker : BackgroundService
    {
        private readonly INetworkService networkService;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInterfaceStatusWorker"/> class.
        /// </summary>
        /// <param name="networkService">The application layer NetworkService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkInterfaceStatusWorker(
            INetworkService networkService,
            IConfiguration configuration,
            ILogger logger)
        {
            this.networkService = networkService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                bool.TryParse(this.configuration[$"Workers:NetworkInterfaceStatus:Enabled"], out var enabled);
                if (!enabled)
                {
                    this.logger.Warn($"NetworkInterfaceStatusWorker is not enabled, returning...");
                    return;
                }

                this.logger.Info($"NetworkInterfaceStatusWorker started");

                var workerInterval = int.Parse(this.configuration["Workers:NetworkInterfaceStatus:Interval"]);
                if (workerInterval <= 0)
                {
                    this.logger.Debug($"NetworkInterfaceStatusWorker has no interval set for recurring task, returning...");
                    return;
                }

                this.logger.Info($"NetworkInterfaceStatusWorker configured to run at interval of {workerInterval} ms");
                while (!stoppingToken.IsCancellationRequested)
                {
                    this.logger.Debug($"NetworkInterfaceStatusWorker running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(workerInterval);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"error running NetworkInterfaceStatusWorker");
            }
            finally
            {
                this.logger.Info($"NetworkInterfaceStatusWorker ended");
            }
        }

        /// <summary>
        /// Retrieves and saves a new value for the network interface status.
        /// </summary>
        /// <param name="samplingInterval">The sampling interval in milliseconds to be used to calculate the status.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected async Task SaveRecurring(int samplingInterval)
        {
            await this.networkService.SaveNetworkInterfacesStatusAsync(samplingInterval);
        }
    }
}
