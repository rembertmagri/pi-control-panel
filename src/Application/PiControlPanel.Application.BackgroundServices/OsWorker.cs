namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using System.Threading;
    using System.Threading.Tasks;

    public class OsWorker : BaseWorker<Os>
    {
        public OsWorker(
            IOsService osService,
            IConfiguration configuration,
            ILogger logger)
            : base(osService, configuration, logger)
        {
        }

        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IOsService)this.service).SaveStatusAsync();
        }
    }
}
