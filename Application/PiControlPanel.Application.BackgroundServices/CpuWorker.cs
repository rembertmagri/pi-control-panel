namespace PiControlPanel.Application.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuWorker : BaseWorker<Cpu>
    {
        public CpuWorker(
            ICpuService cpuService,
            IConfiguration configuration,
            ILogger logger)
            : base(cpuService, configuration, logger)
        {
        }

        protected override async Task SaveRecurring(CancellationToken stoppingToken)
        {
            await ((ICpuService)this.service).SaveLoadStatusAsync();
            await ((ICpuService)this.service).SaveTemperatureAsync();
            await ((ICpuService)this.service).SaveFrequencyAsync();
        }
    }
}
