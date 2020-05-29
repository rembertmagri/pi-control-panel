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
        public OsWorker(
            IOsService operatingSystemService,
            IConfiguration configuration,
            ILogger logger)
            : base(operatingSystemService, configuration, logger)
        {
        }

        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IOsService)this.service).SaveStatusAsync();
        }
    }
}
