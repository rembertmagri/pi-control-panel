namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class ChipsetService : BaseService<Chipset>, IChipsetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipsetService"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public ChipsetService(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        protected override Chipset GetModel()
        {
            var result = BashCommands.CatProcCpuInfo.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatProcCpuInfo}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var version = lines.Last(line => line.StartsWith("Hardware"))
                .Split(':')[1].Trim();
            this.Logger.Trace($"Chipset version: '{version}'");
            var revision = lines.Last(line => line.StartsWith("Revision"))
                .Split(':')[1].Trim();
            this.Logger.Trace($"Chipset revision: '{revision}'");
            var serial = lines.Last(line => line.StartsWith("Serial"))
                .Split(':')[1].Trim();
            this.Logger.Trace($"Chipset serial: '{serial}'");
            var model = lines.Last(line => line.StartsWith("Model"))
                .Split(':')[1].Trim();
            this.Logger.Trace($"Chipset model: '{model}'");

            return new Chipset()
            {
                Version = version,
                Revision = revision,
                Serial = serial,
                Model = model
            };
        }
    }
}
