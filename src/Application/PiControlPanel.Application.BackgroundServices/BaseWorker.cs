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
    public abstract class BaseWorker<T> : BackgroundService
    {
        /// <summary>
        /// The application layer service.
        /// </summary>
        protected readonly IBaseService<T> service;

        /// <summary>
        /// The IConfiguration instance.
        /// </summary>
        protected readonly IConfiguration configuration;

        /// <summary>
        /// The NLog logger instance.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWorker{T}"/> class.
        /// </summary>
        /// <param name="service">The application layer service.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseWorker(
            IBaseService<T> service,
            IConfiguration configuration,
            ILogger logger)
        {
            this.service = service;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Retrieves and saves a new value for the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual Task SaveAsync()
        {
            return this.service.SaveAsync();
        }

        /// <summary>
        /// Retrieves and saves a new value for the timed model.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual Task SaveRecurring(CancellationToken stoppingToken)
        {
            this.logger.Debug($"{this.GetType().Name} has no recurring task, returning...");
            return Task.CompletedTask;
        }
    }
}
