namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class GpuWorker : BaseWorker<Gpu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpuWorker"/> class.
        /// </summary>
        /// <param name="gpuService">The application layer GpuService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public GpuWorker(
            IGpuService gpuService,
            IConfiguration configuration,
            ILogger logger)
            : base(gpuService, configuration, logger)
        {
        }
    }
}
