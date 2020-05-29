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
        public DiskWorker(
            IDiskService diskService,
            IConfiguration configuration,
            ILogger logger)
            : base(diskService, configuration, logger)
        {
        }

        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IDiskService)this.service).SaveFileSystemStatusAsync();
        }
    }
}
