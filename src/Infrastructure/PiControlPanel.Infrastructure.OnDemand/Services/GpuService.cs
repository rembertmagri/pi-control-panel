namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class GpuService : BaseService<Gpu>, IGpuService
    {
        public GpuService(ILogger logger)
            : base(logger)
        {
        }

        protected override Gpu GetModel()
        {
            var result = BashCommands.GetMemGpu.Bash();
            logger.Trace($"Result of '{BashCommands.GetMemGpu}' command: '{result}'");
            string gpu = result.Replace("gpu=", string.Empty).Replace("M", string.Empty);
            logger.Trace($"Gpu memory: '{gpu}' MB");

            var frequency = 500;
            result = BashCommands.CatBootConfig.Bash();
            logger.Trace($"Result of '{BashCommands.CatBootConfig}' command: '{result}'");
            var lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var frequencyLine = lines.FirstOrDefault(line => line.Contains("gpu_freq="));
            var frequencyLineRegex = new Regex(@"^(?<commented>#?)\s*gpu_freq=(?<frequency>\d+)$");

            if (!string.IsNullOrEmpty(frequencyLine))
            {
                logger.Trace($"Frequency line in config file: '{frequencyLine}'");
                var frequencyLineGroups = frequencyLineRegex.Match(frequencyLine).Groups;
                frequency = !string.IsNullOrEmpty(frequencyLineGroups["commented"].Value) ?
                    500 : int.Parse(frequencyLineGroups["frequency"].Value);
            }

            return new Gpu()
            {
                Memory = int.Parse(gpu),
                Frequency = frequency
            };
        }
    }
}
