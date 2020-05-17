namespace PiControlPanel.Application.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    public class CpuFrequencyWorker : BackgroundService
    {
        protected readonly ICpuService cpuService;
        protected readonly IConfiguration configuration;
        protected readonly ILogger logger;

        public CpuFrequencyWorker(
            ICpuService cpuService,
            IConfiguration configuration,
            ILogger logger)
        {
            this.cpuService = cpuService;
            this.configuration = configuration;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.Info($"CpuFrequencyWorker started");

                var workerInterval = int.Parse(configuration["Worker:Interval"]);
                logger.Info($"CpuFrequencyWorker configured to run at interval of {workerInterval} ms");

                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.Debug($"CpuFrequencyWorker running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(workerInterval);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"error running CpuFrequencyWorker");
            }
            finally
            {
                logger.Info($"CpuFrequencyWorker ended");
            }
        }

        protected async Task SaveRecurring(int samplingInterval)
        {
            await this.cpuService.SaveFrequencyAsync(samplingInterval);
        }
    }
}
