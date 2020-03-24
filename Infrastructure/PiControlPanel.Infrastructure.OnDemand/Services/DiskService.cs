namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public class DiskService : BaseService<Disk>, IDiskService
    {
        private readonly ISubject<DiskStatus> diskStatusSubject;

        public DiskService(ISubject<DiskStatus> diskStatusSubject, ILogger logger)
            : base(logger)
        {
            this.diskStatusSubject = diskStatusSubject;
        }

        public Task<DiskStatus> GetStatusAsync()
        {
            logger.Info("Infra layer -> DiskService -> GetStatusAsync");
            var diskStatus = this.GetDiskStatus();
            return Task.FromResult(diskStatus);
        }

        public IObservable<DiskStatus> GetStatusObservable()
        {
            logger.Info("Infra layer -> DiskService -> GetStatusObservable");
            return this.diskStatusSubject.AsObservable();
        }

        public void PublishStatus(DiskStatus status)
        {
            logger.Info("Infra layer -> DiskService -> PublishStatus");
            this.diskStatusSubject.OnNext(status);
        }

        protected override Disk GetModel()
        {
            var result = BashCommands.Df.Bash();
            logger.Debug($"Result of '{BashCommands.Df}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var diskInfo = lines.First(l => l.StartsWith("/dev/") && l.Contains("ext4"));
            var regex = new Regex(@"^(?<filesystem>\S*)\s*(?<type>\S*)\s*(?<total>\S*)\s*(?<used>\S*)\s*(?<free>\S*).*$");
            var groups = regex.Match(diskInfo).Groups;

            return new Disk()
            {
                FileSystem = groups["filesystem"].Value,
                Type = groups["type"].Value,
                Total = int.Parse(groups["total"].Value)
            };
        }

        private DiskStatus GetDiskStatus()
        {
            var result = BashCommands.Df.Bash();
            logger.Debug($"Result of '{BashCommands.Df}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var diskInfo = lines.First(l => l.StartsWith("/dev/") && l.Contains("ext4"));
            var regex = new Regex(@"^(?<filesystem>\S*)\s*(?<type>\S*)\s*(?<total>\S*)\s*(?<used>\S*)\s*(?<free>\S*).*$");
            var groups = regex.Match(diskInfo).Groups;
            return new DiskStatus()
            {
                Used = int.Parse(groups["used"].Value),
                Available = int.Parse(groups["free"].Value),
                DateTime = DateTime.Now
            };
        }
    }
}
