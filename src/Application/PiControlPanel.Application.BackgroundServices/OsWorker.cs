namespace PiControlPanel.Application.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;

    /// <inheritdoc/>
    public class OsWorker : BaseWorker<Os>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsWorker"/> class.
        /// </summary>
        /// <param name="operatingSystemService">The application layer OsService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsWorker(
            IOsService operatingSystemService,
            IConfiguration configuration,
            ILogger logger)
            : base(operatingSystemService, configuration, logger)
        {
        }

        /// <inheritdoc/>
        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IOsService)this.Service).SaveStatusAsync();
        }
    }
}
