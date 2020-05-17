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

    public class OsService : BaseService<Os>, IOsService
    {
        private readonly ISubject<OsStatus> osStatusSubject;

        public OsService(ISubject<OsStatus> osStatusSubject, ILogger logger)
            : base(logger)
        {
            this.osStatusSubject = osStatusSubject;
        }

        public Task<OsStatus> GetStatusAsync()
        {
            logger.Debug("Infra layer -> OsService -> GetStatusAsync");
            var osStatus = this.GetOsStatus();
            return Task.FromResult(osStatus);
        }

        public IObservable<OsStatus> GetStatusObservable()
        {
            logger.Debug("Infra layer -> OsService -> GetStatusObservable");
            return this.osStatusSubject.AsObservable();
        }

        public void PublishStatus(OsStatus status)
        {
            logger.Debug("Infra layer -> OsService -> PublishStatus");
            this.osStatusSubject.OnNext(status);
        }

        protected override Os GetModel()
        {
            var result = BashCommands.Hostnamectl.Bash();
            logger.Trace($"Result of '{BashCommands.Hostnamectl}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var hostnameInfo = lines.First(l => l.Contains("Static hostname:"));
            var hostname = hostnameInfo.Replace("Static hostname:", string.Empty).Trim();
            logger.Trace($"Hostname: '{hostname}'");

            var osInfo = lines.First(l => l.Contains("Operating System:"));
            var os = osInfo.Replace("Operating System:", string.Empty).Trim();
            logger.Trace($"Operating System Name: '{os}'");

            var kernelInfo = lines.First(l => l.Contains("Kernel:"));
            var kernel = kernelInfo.Replace("Kernel:", string.Empty).Trim();
            logger.Trace($"Kernel: '{kernel}'");

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
            logger.Trace($"Result of '{BashCommands.Uptime}' command: '{result}'");

            var uptimeResult = result.Replace("up ", string.Empty);
            logger.Trace($"Uptime substring: '{uptimeResult}'");

            return new OsStatus()
            {
                Uptime = uptimeResult,
                DateTime = DateTime.Now
            };
        }
    }
}
