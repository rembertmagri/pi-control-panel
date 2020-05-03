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
                logger.Info($"BaseWorker<{typeof(T).Name}> started");
                await this.SaveAsync();

                var workerInterval = int.Parse(configuration["Worker:Interval"]);
                logger.Info($"BaseWorker<{typeof(T).Name}> configured to run at interval of {workerInterval} ms");

                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.Trace($"BaseWorker<{typeof(T).Name}> running at: {DateTimeOffset.Now}");
                    await this.SaveRecurring(stoppingToken);
                    await Task.Delay(workerInterval, stoppingToken);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"error running BaseWorker<{typeof(T).Name}>");
            }
            finally
            {
                logger.Info($"BaseWorker<{typeof(T).Name}> ended");
            }
        }

        protected virtual Task SaveAsync()
        {
            return this.service.SaveAsync();
        }

        protected virtual Task SaveRecurring(CancellationToken stoppingToken)
        {
            logger.Trace($"BaseWorker<{typeof(T).Name}> has no recurring task, returning...");
            return Task.CompletedTask;
        }
    }
}
