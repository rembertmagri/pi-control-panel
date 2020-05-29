namespace PiControlPanel.Application.BackgroundServices
{
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class ChipsetWorker : BaseWorker<Chipset>
    {
        public ChipsetWorker(
            IChipsetService chipsetService,
            IConfiguration configuration,
            ILogger logger)
            : base(chipsetService, configuration, logger)
        {
        }
    }
}
