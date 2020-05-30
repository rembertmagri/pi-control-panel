namespace PiControlPanel.Application.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <inheritdoc/>
    public class MemoryWorker<TMemory, TMemoryStatus> : BaseWorker<TMemory>
        where TMemory : Memory
        where TMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryWorker{TMemory, TMemoryStatus}"/> class.
        /// </summary>
        /// <param name="memoryService">The application layer MemoryService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public MemoryWorker(
            IMemoryService<TMemory, TMemoryStatus> memoryService,
            IConfiguration configuration,
            ILogger logger)
            : base(memoryService, configuration, logger)
        {
        }

        /// <inheritdoc/>
        protected override Task SaveRecurring(CancellationToken stoppingToken)
        {
            return ((IMemoryService<TMemory, TMemoryStatus>)this.Service).SaveStatusAsync();
        }
    }
}
