namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware;

    /// <inheritdoc/>
    public class GpuService : BaseService<Gpu>, IGpuService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpuService"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public GpuService(ILogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        protected override Gpu GetModel()
        {
            var result = BashCommands.GetMemGpu.Bash();
            this.Logger.Trace($"Result of '{BashCommands.GetMemGpu}' command: '{result}'");
            string gpu = result.Replace("gpu=", string.Empty).Replace("M", string.Empty);
            this.Logger.Trace($"Gpu memory: '{gpu}' MB");

            var frequency = 500;
            result = BashCommands.CatBootConfig.Bash();
            this.Logger.Trace($"Result of '{BashCommands.CatBootConfig}' command: '{result}'");
            var lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var frequencyLine = lines.FirstOrDefault(line => line.Contains("gpu_freq="));
            var frequencyLineRegex = new Regex(@"^(?<commented>#?)\s*gpu_freq=(?<frequency>\d+)$");

            if (!string.IsNullOrEmpty(frequencyLine))
            {
                this.Logger.Trace($"Frequency line in config file: '{frequencyLine}'");
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
