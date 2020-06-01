﻿namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;

    /// <inheritdoc/>
    public class ControlPanelService : IControlPanelService
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelService"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelService(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<bool> RebootAsync()
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> RebootAsync");
            var result = BashCommands.SudoReboot.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoReboot}' command: '{result}'");
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> ShutdownAsync()
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> ShutdownAsync");
            var result = BashCommands.SudoShutdown.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoShutdown}' command: '{result}'");
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> UpdateAsync()
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> UpdateAsync");
            var result = BashCommands.SudoAptgetUpdade.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoAptgetUpdade}' command: '{result}'");
            result = BashCommands.SudoAptgetUpgrade.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoAptgetUpgrade}' command: '{result}'");

            string lastLine = result
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();
            this.logger.Info($"Firmware update summary: '{lastLine}'");
            if ("0 upgraded, 0 newly installed, 0 to remove and 0 not upgraded."
                .Equals(lastLine))
            {
                this.logger.Info("Firmware already up-to-date, no need to update.");
                return Task.FromResult(false);
            }

            result = BashCommands.SudoAptgetAutoremove.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoAptgetAutoremove}' command: '{result}'");
            result = BashCommands.SudoAptgetAutoclean.Bash();
            this.logger.Trace($"Result of '{BashCommands.SudoAptgetAutoclean}' command: '{result}'");

            return this.RebootAsync();
        }

        /// <inheritdoc/>
        public Task<bool> KillAsync(UserContext context, int processId)
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> KillAsync");

            var sudoKillCommand = string.Format(BashCommands.SudoKill, processId);
            var result = sudoKillCommand.Bash();

            if (!string.IsNullOrEmpty(result))
            {
                this.logger.Warn($"Result of '{sudoKillCommand}' command: '{result}'");
                return Task.FromResult(false);
            }

            this.logger.Info($"Result of '{sudoKillCommand}' command is empty, success");
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<string> GetProcessOwnerUsernameAsync(int processId)
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> GetProcessOwnerUsernameAsync");

            var processStatusUserCommand = string.Format(BashCommands.PsUser, processId);
            var result = processStatusUserCommand.Bash();

            if (string.IsNullOrEmpty(result))
            {
                this.logger.Warn($"Result of '{processStatusUserCommand}' command is empty, process '{processId}' doesn't exist");
                return Task.FromResult(string.Empty);
            }

            this.logger.Trace($"Result of '{processStatusUserCommand}' command: '{result}'");
            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel)
        {
            this.logger.Debug("Infra layer -> ControlPanelService -> OverclockAsync");

            var result = BashCommands.CatBootConfig.Bash();
            this.logger.Trace($"Result of '{BashCommands.CatBootConfig}' command: '{result}'");
            var lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var frequencyLine = lines.FirstOrDefault(line => line.Contains("arm_freq="));
            var frequencyLineRegex = new Regex(@"^(?<commented>#?)\s*arm_freq=(?<frequency>\d+)$");
            this.logger.Trace($"Frequency line in config file: '{frequencyLine}'");
            var frequencyLineGroups = frequencyLineRegex.Match(frequencyLine).Groups;
            var currentFrequency = !string.IsNullOrEmpty(frequencyLineGroups["commented"].Value) ?
                1500 : int.Parse(frequencyLineGroups["frequency"].Value);

            if (currentFrequency == (int)cpuMaxFrequencyLevel)
            {
                this.logger.Info($"Frequency already set to {currentFrequency}, no need to restart");
                return Task.FromResult(false);
            }

            var overVoltageLine = lines.FirstOrDefault(line => line.Contains("over_voltage="));

            if (string.IsNullOrEmpty(overVoltageLine))
            {
                this.logger.Info($"over_voltage configuration didn't exist, creating...");
                var createOverVoltageConfigCommand = string.Format(
                    BashCommands.SudoSedBootConfig,
                    frequencyLine,
                    $"#over_voltage=0\\n{frequencyLine}");
                result = createOverVoltageConfigCommand.Bash();
                if (!string.IsNullOrEmpty(result))
                {
                    this.logger.Error($"Result of '{createOverVoltageConfigCommand}' command: '{result}', couldn't create over_voltage configuration");
                    throw new BusinessException("Couldn't create over_voltage configuration");
                }

                this.logger.Debug($"Result of '{createOverVoltageConfigCommand}' command is empty, success");
                overVoltageLine = "#over_voltage=0";
            }

            var overVoltage = 0;
            var frequency = 1500;
            switch (cpuMaxFrequencyLevel)
            {
                case CpuMaxFrequencyLevel.Default:
                    break;
                case CpuMaxFrequencyLevel.High:
                    overVoltage = 2;
                    frequency = 1750;
                    break;
                case CpuMaxFrequencyLevel.Maximum:
                    overVoltage = 6;
                    frequency = 2000;
                    break;
                default:
                    throw new BusinessException($"Invalid value for cpu frequency level: {cpuMaxFrequencyLevel}");
            }

            var setOverVoltageConfigCommand = string.Format(
                BashCommands.SudoSedBootConfig,
                $"{overVoltageLine}",
                $"over_voltage={overVoltage}");
            result = setOverVoltageConfigCommand.Bash();
            if (!string.IsNullOrEmpty(result))
            {
                this.logger.Error($"Result of '{setOverVoltageConfigCommand}' command: '{result}', couldn't set over_voltage configuration");
                throw new BusinessException("Couldn't set over_voltage configuration");
            }

            this.logger.Debug($"Result of '{setOverVoltageConfigCommand}' command is empty, success");

            var setFrequencyConfigCommand = string.Format(
                BashCommands.SudoSedBootConfig,
                $"{frequencyLine}",
                $"arm_freq={frequency}");
            result = setFrequencyConfigCommand.Bash();
            if (!string.IsNullOrEmpty(result))
            {
                this.logger.Error($"Result of '{setFrequencyConfigCommand}' command: '{result}', couldn't set over_voltage configuration");
                throw new BusinessException("Couldn't set arm_freq configuration");
            }

            this.logger.Debug($"Result of '{setFrequencyConfigCommand}' command is empty, success");

            return this.RebootAsync();
        }
    }
}
