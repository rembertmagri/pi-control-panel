namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware.Os;

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
            var operatingSystemStatus = this.GetOsStatus();
            return Task.FromResult(operatingSystemStatus);
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
        protected override Os GetModel()
        {
            var result = BashCommands.Hostnamectl.Bash();
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

            return new Os()
            {
                Name = os,
                Kernel = kernel,
                Hostname = hostname
            };
        }

        private OsStatus GetOsStatus()
        {
            var result = BashCommands.Uptime.Bash();
            this.Logger.Trace($"Result of '{BashCommands.Uptime}' command: '{result}'");
            var uptimeResult = result.Replace("up ", string.Empty);
            this.Logger.Trace($"Uptime substring: '{uptimeResult}'");

            result = BashCommands.SudoAptGetUpdate.Bash();
            this.Logger.Trace($"Result of '{BashCommands.SudoAptGetUpdate}' command: '{result}'");
            var aptGetUpgradeSimulateCommand = string.Format(BashCommands.SudoAptGetUpgrade, "s");
            result = aptGetUpgradeSimulateCommand.Bash();
            this.Logger.Trace($"Result of '{aptGetUpgradeSimulateCommand}' command: '{result}'");
            var lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var upgradeablePackagesSummary = lines.Single(line => line.EndsWith(" not upgraded."));
            if (!int.TryParse(
                upgradeablePackagesSummary.Substring(0, upgradeablePackagesSummary.IndexOf(" ") + 1),
                out var upgradeablePackages))
            {
                this.Logger.Warn($"Could not parse upgradeable packages: '{upgradeablePackagesSummary}'");
            }

            return new OsStatus()
            {
                Uptime = uptimeResult,
                UpgradeablePackages = upgradeablePackages,
                DateTime = DateTime.Now
            };
        }
    }
}
