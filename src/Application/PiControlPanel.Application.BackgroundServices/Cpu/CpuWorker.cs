namespace PiControlPanel.Application.BackgroundServices.Cpu
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <inheritdoc/>
    public class CpuWorker : BaseWorker<Cpu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuWorker"/> class.
        /// </summary>
        /// <param name="cpuService">The application layer CpuService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuWorker(
            ICpuService cpuService,
            IConfiguration configuration,
            ILogger logger)
            : base(cpuService, configuration, logger)
        {
        }

        /// <inheritdoc/>
        protected override async Task SaveRecurring(CancellationToken stoppingToken)
        {
            await ((ICpuService)this.Service).SaveLoadStatusAsync();
            await ((ICpuService)this.Service).SaveSensorsStatusAsync();
        }
    }
}
