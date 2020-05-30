namespace PiControlPanel.Application.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <inheritdoc/>
    public class DiskWorker : BaseWorker<Disk>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskWorker"/> class.
        /// </summary>
        /// <param name="diskService">The application layer DiskService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskWorker(
            IDiskService diskService,
            IConfiguration configuration,
            ILogger logger)
            : base(diskService, configuration, logger)
        {
        }

        /// <inheritdoc/>
        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IDiskService)this.service).SaveFileSystemStatusAsync();
        }
    }
}
