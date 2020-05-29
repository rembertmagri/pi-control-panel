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

        public OsService(ISubject<OsStatus> operatingSystemStatusSubject, ILogger logger)
            : base(logger)
        {
            this.operatingSystemStatusSubject = operatingSystemStatusSubject;
        }

        public Task<OsStatus> GetStatusAsync()
        {
            this.logger.Debug("Infra layer -> OsService -> GetStatusAsync");
            var operatingSystemStatus = this.GetOsStatus();
            return Task.FromResult(operatingSystemStatus);
        }

        public IObservable<OsStatus> GetStatusObservable()
        {
            this.logger.Debug("Infra layer -> OsService -> GetStatusObservable");
            return this.operatingSystemStatusSubject.AsObservable();
        }

        public void PublishStatus(OsStatus status)
        {
            this.logger.Debug("Infra layer -> OsService -> PublishStatus");
            this.operatingSystemStatusSubject.OnNext(status);
        }

        protected override Os GetModel()
        {
            var result = BashCommands.Hostnamectl.Bash();
            this.logger.Trace($"Result of '{BashCommands.Hostnamectl}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var hostnameInfo = lines.First(l => l.Contains("Static hostname:"));
            var hostname = hostnameInfo.Replace("Static hostname:", string.Empty).Trim();
            this.logger.Trace($"Hostname: '{hostname}'");

            var operatingSystemInfo = lines.First(l => l.Contains("Operating System:"));
            var os = operatingSystemInfo.Replace("Operating System:", string.Empty).Trim();
            this.logger.Trace($"Operating System Name: '{os}'");

            var kernelInfo = lines.First(l => l.Contains("Kernel:"));
            var kernel = kernelInfo.Replace("Kernel:", string.Empty).Trim();
            this.logger.Trace($"Kernel: '{kernel}'");

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
            this.logger.Trace($"Result of '{BashCommands.Uptime}' command: '{result}'");

            var uptimeResult = result.Replace("up ", string.Empty);
            this.logger.Trace($"Uptime substring: '{uptimeResult}'");

            return new OsStatus()
            {
                Uptime = uptimeResult,
                DateTime = DateTime.Now
            };
        }
    }
}
