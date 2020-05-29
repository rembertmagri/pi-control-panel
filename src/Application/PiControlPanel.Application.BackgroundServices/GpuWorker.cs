namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class GpuWorker : BaseWorker<Gpu>
    {
        public GpuWorker(
            IGpuService gpuService,
            IConfiguration configuration,
            ILogger logger)
            : base(gpuService, configuration, logger)
        {
        }
    }
}
