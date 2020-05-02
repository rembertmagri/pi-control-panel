namespace PiControlPanel.Application.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    public class NetworkInterfaceStatusWorker : BackgroundService
    {
        protected readonly INetworkService networkService;
        protected readonly IConfiguration configuration;
        protected readonly ILogger logger;

        public NetworkInterfaceStatusWorker(
            INetworkService networkService,
            IConfiguration configuration,
            ILogger logger)
        {
            this.networkService = networkService;
            this.configuration = configuration;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.Info($"NetworkInterfaceStatusWorker started");

                var workerInterval = int.Parse(configuration["Worker:Interval"]);
                logger.Info($"NetworkInterfaceStatusWorker configured to run at interval of {workerInterval} ms");

                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.Trace($"NetworkInterfaceStatusWorker running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(workerInterval);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"error running NetworkInterfaceStatusWorker");
            }
            finally
            {
                logger.Info($"NetworkInterfaceStatusWorker ended");
            }
        }

        protected async Task SaveRecurring(int samplingInterval)
        {
            await this.networkService.SaveNetworkInterfacesStatusAsync(samplingInterval);
        }
    }
}
