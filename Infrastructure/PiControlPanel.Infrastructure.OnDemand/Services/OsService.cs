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
            logger.Trace("Infra layer -> OsService -> GetStatusAsync");
            var diskStatus = this.GetOsStatus();
            return Task.FromResult(diskStatus);
        }

        public IObservable<OsStatus> GetStatusObservable()
        {
            logger.Trace("Infra layer -> OsService -> GetStatusObservable");
            return this.osStatusSubject.AsObservable();
        }

        public void PublishStatus(OsStatus status)
        {
            logger.Trace("Infra layer -> OsService -> PublishStatus");
            this.osStatusSubject.OnNext(status);
        }

        protected override Os GetModel()
        {
            var result = BashCommands.Hostnamectl.Bash();
            logger.Debug($"Result of '{BashCommands.Hostnamectl}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var hostnameInfo = lines.First(l => l.Contains("Static hostname:"));
            var hostname = hostnameInfo.Replace("Static hostname:", string.Empty).Trim();
            logger.Debug($"Hostname: '{hostname}'");

            var osInfo = lines.First(l => l.Contains("Operating System:"));
            var os = osInfo.Replace("Operating System:", string.Empty).Trim();
            logger.Debug($"Operating System Name: '{os}'");

            var kernelInfo = lines.First(l => l.Contains("Kernel:"));
            var kernel = kernelInfo.Replace("Kernel:", string.Empty).Trim();
            logger.Debug($"Kernel: '{kernel}'");

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
            logger.Debug($"Result of '{BashCommands.Uptime}' command: '{result}'");

            var uptimeResult = result.Replace("up ", string.Empty);
            logger.Debug($"Uptime substring: '{uptimeResult}'");

            return new OsStatus()
            {
                Uptime = uptimeResult,
                DateTime = DateTime.Now
            };
        }
    }
}
