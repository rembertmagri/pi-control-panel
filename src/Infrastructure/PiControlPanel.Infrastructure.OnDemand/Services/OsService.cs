﻿namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Infrastructure.OnDemand.Util;

    /// <inheritdoc/>
    public class OsService : BaseService<Os>, IOsService
    {
        private readonly ISubject<OsStatus> operatingSystemStatusSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsService"/> class.
        /// </summary>
        /// <param name="operatingSystemStatusSubject">The operating system status subject.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsService(ISubject<OsStatus> operatingSystemStatusSubject, ILogger logger)
            : base(logger)
        {
            this.operatingSystemStatusSubject = operatingSystemStatusSubject;
        }

        /// <inheritdoc/>
        public Task<OsStatus> GetStatusAsync()
        {
            this.Logger.Debug("Infra layer -> OsService -> GetStatusAsync");
            return this.GetOsStatusAsync();
        }

        /// <inheritdoc/>
        public IObservable<OsStatus> GetStatusObservable()
        {
            this.Logger.Debug("Infra layer -> OsService -> GetStatusObservable");
            return this.operatingSystemStatusSubject.AsObservable();
        }

        /// <inheritdoc/>
        public void PublishStatus(OsStatus status)
        {
            this.Logger.Debug("Infra layer -> OsService -> PublishStatus");
            this.operatingSystemStatusSubject.OnNext(status);
        }

        /// <inheritdoc/>
        protected override async Task<Os> GetModelAsync()
        {
            var result = await BashCommands.Hostnamectl.BashAsync();
            this.Logger.Trace($"Result of '{BashCommands.Hostnamectl}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var hostnameInfo = lines.First(l => l.Contains("Static hostname:"));
            var hostname = hostnameInfo.Replace("Static hostname:", string.Empty).Trim();
            this.Logger.Trace($"Hostname: '{hostname}'");

            var operatingSystemInfo = lines.First(l => l.Contains("Operating System:"));
            var os = operatingSystemInfo.Replace("Operating System:", string.Empty).Trim();
            this.Logger.Trace($"Operating System Name: '{os}'");

            var kernelInfo = lines.First(l => l.Contains("Kernel:"));
            var kernel = kernelInfo.Replace("Kernel:", string.Empty).Trim();
            this.Logger.Trace($"Kernel: '{kernel}'");

            result = await BashCommands.CatSshdConfig.BashAsync();
            this.Logger.Trace($"Result of '{BashCommands.CatSshdConfig}' command: '{result}'");
            lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var portLine = lines.SingleOrDefault(line => line.StartsWith("Port "));
            var sshPort = string.IsNullOrEmpty(portLine) ? 22 : int.Parse(portLine.Replace("Port ", string.Empty));
            var netstat = string.Format(BashCommands.Netstat, sshPort);
            var sshStarted = false;
            try
            {
                result = await netstat.BashAsync();
                this.Logger.Trace($"Result of '{netstat}' command: '{result}'");
                sshStarted = !string.IsNullOrEmpty(result);
            }
            catch (BusinessException ex)
            {
                this.Logger.Error(ex, $"Error running '{netstat}' command");
            }

            var upgradeablePackages = 0;
            var systemClockSynchronized = await this.WaitSystemClockSynchronized(60000);
            if (systemClockSynchronized)
            {
                result = await BashCommands.SudoAptGetUpdate.BashAsync();
                this.Logger.Trace($"Result of '{BashCommands.SudoAptGetUpdate}' command: '{result}'");
                var aptGetUpgradeSimulateCommand = string.Format(BashCommands.SudoAptGetUpgrade, "s");
                result = await aptGetUpgradeSimulateCommand.BashAsync();
                this.Logger.Trace($"Result of '{aptGetUpgradeSimulateCommand}' command: '{result}'");
                lines = result.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);
                var upgradeablePackagesSummary = lines.Single(line => line.EndsWith(" not upgraded."));
                if (!int.TryParse(
                    upgradeablePackagesSummary.Substring(0, upgradeablePackagesSummary.IndexOf(" ") + 1),
                    out upgradeablePackages))
                {
                    this.Logger.Warn($"Could not parse upgradeable packages: '{upgradeablePackagesSummary}'");
                }
            }

            return new Os()
            {
                Name = os,
                Kernel = kernel,
                Hostname = hostname,
                SshStarted = sshStarted,
                SshPort = sshPort,
                UpgradeablePackages = upgradeablePackages
            };
        }

        private async Task<OsStatus> GetOsStatusAsync()
        {
            var result = await BashCommands.Uptime.BashAsync();
            this.Logger.Trace($"Result of '{BashCommands.Uptime}' command: '{result}'");
            var uptimeResult = result.Replace("up ", string.Empty);
            this.Logger.Trace($"Uptime substring: '{uptimeResult}'");

            return new OsStatus()
            {
                Uptime = uptimeResult,
                DateTime = DateTime.Now
            };
        }

        private async Task<bool> WaitSystemClockSynchronized(int timeoutInMilliseconds)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeoutInMilliseconds)
            {
                var result = await BashCommands.Timedatectl.BashAsync();
                this.Logger.Trace($"Result of '{BashCommands.Timedatectl}' command: '{result}'");
                var lines = result.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);
                var systemClockSynchronizedLine = lines.Single(line => line.StartsWith("System clock synchronized: "));
                var systemClockSynchronized = systemClockSynchronizedLine.Replace("System clock synchronized: ", string.Empty);
                if ("yes".Equals(systemClockSynchronized))
                {
                    stopWatch.Stop();
                    this.Logger.Info($"System clock synchronized after {stopWatch.Elapsed.TotalMilliseconds} ms");
                    return true;
                }

                await Task.Delay(15000);
            }

            stopWatch.Stop();
            this.Logger.Info($"System clock not synchronized after {stopWatch.Elapsed.TotalMilliseconds} ms");
            return false;
        }
    }
}
