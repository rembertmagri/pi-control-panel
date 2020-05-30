namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class ChipsetWorker : BaseWorker<Chipset>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipsetWorker"/> class.
        /// </summary>
        /// <param name="chipsetService">The application layer ChipsetService.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ChipsetWorker(
            IChipsetService chipsetService,
            IConfiguration configuration,
            ILogger logger)
            : base(chipsetService, configuration, logger)
        {
        }
    }
}
