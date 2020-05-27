namespace PiControlPanel.Application.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;

    public abstract class BaseWorker<T> : BackgroundService
    {
        protected readonly IBaseService<T> service;
        protected readonly IConfiguration configuration;
        protected readonly ILogger logger;

        public BaseWorker(
            IBaseService<T> service,
            IConfiguration configuration,
            ILogger logger)
        {
            this.service = service;
            this.configuration = configuration;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                bool.TryParse(this.configuration[$"Workers:{typeof(T).Name}:Enabled"], out var enabled);
                if (!enabled)
                {
                    this.logger.Warn($"{this.GetType().Name} is not enabled, returning...");
                    return;
                }

                this.logger.Info($"{this.GetType().Name} started");
                await this.SaveAsync();

                var workerInterval = int.Parse(this.configuration[$"Workers:{typeof(T).Name}:Interval"]);
                if (workerInterval <= 0)
                {
                    this.logger.Debug($"{this.GetType().Name} has no interval set for recurring task, returning...");
                    return;
                }

                this.logger.Info($"{this.GetType().Name} configured to run at interval of {workerInterval} ms");
                while (!stoppingToken.IsCancellationRequested)
                {
                    this.logger.Debug($"{this.GetType().Name} running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(stoppingToken);
                    await Task.Delay(workerInterval, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"error running {this.GetType().Name}");
            }
            finally
            {
                this.logger.Info($"{this.GetType().Name} ended");
            }
        }

        protected virtual Task SaveAsync()
        {
            return this.service.SaveAsync();
        }

        protected virtual Task SaveRecurring(CancellationToken stoppingToken)
        {
            this.logger.Debug($"{this.GetType().Name} has no recurring task, returning...");
            return Task.CompletedTask;
        }
    }
}
