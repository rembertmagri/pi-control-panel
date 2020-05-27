namespace PiControlPanel.Application.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryWorker<T, U> : BaseWorker<T>
        where T : Memory
        where U : MemoryStatus
    {
        public MemoryWorker(
            IMemoryService<T, U> memoryService,
            IConfiguration configuration,
            ILogger logger)
            : base(memoryService, configuration, logger)
        {
        }

        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IMemoryService<T, U>)this.service).SaveStatusAsync();
        }
    }
}
