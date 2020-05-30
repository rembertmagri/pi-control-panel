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
    public class CpuFrequencyWorker : BackgroundService
    {
        private readonly ICpuService cpuService;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuFrequencyWorker"/> class.
        /// </summary>
        /// <param name="cpuService">The application layer CpuService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuFrequencyWorker(
            ICpuService cpuService,
            IConfiguration configuration,
            ILogger logger)
        {
            this.cpuService = cpuService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                bool.TryParse(this.configuration[$"Workers:CpuFrequency:Enabled"], out var enabled);
                if (!enabled)
                {
                    this.logger.Warn($"CpuFrequencyWorker is not enabled, returning...");
                    return;
                }

                this.logger.Info($"CpuFrequencyWorker started");

                var workerInterval = int.Parse(this.configuration["Workers:CpuFrequency:Interval"]);
                if (workerInterval <= 0)
                {
                    this.logger.Debug($"CpuFrequencyWorker has no interval set for recurring task, returning...");
                    return;
                }

                this.logger.Info($"CpuFrequencyWorker configured to run at interval of {workerInterval} ms");
                while (!stoppingToken.IsCancellationRequested)
                {
                    this.logger.Debug($"CpuFrequencyWorker running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(workerInterval);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"error running CpuFrequencyWorker");
            }
            finally
            {
                this.logger.Info($"CpuFrequencyWorker ended");
            }
        }

        /// <summary>
        /// Retrieves and saves a new value for the CPU frequency.
        /// </summary>
        /// <param name="samplingInterval">The sampling interval in milliseconds to be used to calculate the frequency.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected async Task SaveRecurring(int samplingInterval)
        {
            await this.cpuService.SaveFrequencyAsync(samplingInterval);
        }
    }
}
