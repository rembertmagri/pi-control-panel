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

    public class ChipsetService : BaseService<Chipset>, IChipsetService
    {
        public ChipsetService(ILogger logger)
            : base(logger)
        {
        }

        protected override Chipset GetModel()
        {
            var result = BashCommands.CatProcCpuInfo.Bash();
            this.logger.Trace($"Result of '{BashCommands.CatProcCpuInfo}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var version = lines.Last(line => line.StartsWith("Hardware"))
                .Split(':')[1].Trim();
            this.logger.Trace($"Chipset version: '{version}'");
            var revision = lines.Last(line => line.StartsWith("Revision"))
                .Split(':')[1].Trim();
            this.logger.Trace($"Chipset revision: '{revision}'");
            var serial = lines.Last(line => line.StartsWith("Serial"))
                .Split(':')[1].Trim();
            this.logger.Trace($"Chipset serial: '{serial}'");
            var model = lines.Last(line => line.StartsWith("Model"))
                .Split(':')[1].Trim();
            this.logger.Trace($"Chipset model: '{model}'");

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
