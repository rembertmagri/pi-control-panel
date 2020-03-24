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
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryService : BaseService<Memory>, IMemoryService
    {
        private readonly ISubject<MemoryStatus> memoryStatusSubject;

        public MemoryService(ISubject<MemoryStatus> memoryStatusSubject, ILogger logger)
            : base(logger)
        {
            this.memoryStatusSubject = memoryStatusSubject;
        }

        public Task<MemoryStatus> GetStatusAsync()
        {
            logger.Info("Infra layer -> MemoryService -> GetStatusAsync");
            var memoryStatus = this.GetMemoryStatus();
            return Task.FromResult(memoryStatus);
        }

        public IObservable<MemoryStatus> GetStatusObservable()
        {
            logger.Info("Infra layer -> MemoryService -> GetStatusObservable");
            return this.memoryStatusSubject.AsObservable();
        }

        public void PublishStatus(MemoryStatus status)
        {
            logger.Info("Infra layer -> MemoryService -> PublishStatus");
            this.memoryStatusSubject.OnNext(status);
        }

        protected override Memory GetModel()
        {
            var result = BashCommands.Free.Bash();
            logger.Debug($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var memoryInfo = lines.First(l => l.StartsWith("Mem:"));
            var regex = new Regex(@"^Mem:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var total = int.Parse(groups["total"].Value);
            logger.Debug($"Total memory: '{total}'KB");
            return new Memory()
            {
                Total = total
            };
        }

        private MemoryStatus GetMemoryStatus()
        {
            var result = BashCommands.Free.Bash();
            logger.Debug($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var memoryInfo = lines.First(l => l.StartsWith("Mem:"));
            var regex = new Regex(@"^Mem:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var used = int.Parse(groups["used"].Value);
            var free = int.Parse(groups["free"].Value);
            logger.Debug($"Used memory: '{used}'KB, Free memory: '{free}'KB");
            return new MemoryStatus()
            {
                Used = used,
                Available = free,
                DateTime = DateTime.Now
            };
        }
    }
}
