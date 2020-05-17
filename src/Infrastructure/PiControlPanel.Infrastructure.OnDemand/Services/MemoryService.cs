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

    public class MemoryService<T, U> : BaseService<T>, IMemoryService<T, U>
        where T : Memory, new()
        where U : MemoryStatus, new()
    {
        private readonly ISubject<U> memoryStatusSubject;

        public MemoryService(ISubject<U> memoryStatusSubject, ILogger logger)
            : base(logger)
        {
            this.memoryStatusSubject = memoryStatusSubject;
        }

        public Task<U> GetStatusAsync()
        {
            logger.Debug("Infra layer -> MemoryService -> GetStatusAsync");
            var memoryStatus = this.GetMemoryStatus();
            return Task.FromResult(memoryStatus);
        }

        public IObservable<U> GetStatusObservable()
        {
            logger.Debug("Infra layer -> MemoryService -> GetStatusObservable");
            return this.memoryStatusSubject.AsObservable();
        }

        public void PublishStatus(U status)
        {
            logger.Debug("Infra layer -> MemoryService -> PublishStatus");
            this.memoryStatusSubject.OnNext(status);
        }

        protected override T GetModel()
        {
            var result = BashCommands.Free.Bash();
            logger.Trace($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var memoryTypeName = typeof(T) == typeof(RandomAccessMemory) ? "Mem:" : "Swap:";
            var memoryInfo = lines.First(l => l.StartsWith(memoryTypeName));
            var regex = new Regex(@"^\w*:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*(?<shared>\d*)\s*(?<buffcache>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var total = int.Parse(groups["total"].Value);
            logger.Trace($"Total memory: '{total}'KB");

            return new T()
            {
                Total = total
            };
        }

        private U GetMemoryStatus()
        {
            var result = BashCommands.Free.Bash();
            logger.Trace($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var isRam = typeof(T) == typeof(RandomAccessMemory);
            var memoryTypeName = isRam ? "Mem:" : "Swap:";
            var memoryInfo = lines.First(l => l.StartsWith(memoryTypeName));
            var regex = isRam ? 
                new Regex(@"^Mem:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*(?<shared>\d*)\s*(?<buffCache>\d*)\s*.*$") :
                new Regex(@"^Swap:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var used = int.Parse(groups["used"].Value);
            var free = int.Parse(groups["free"].Value);
            logger.Trace($"Used memory: '{used}'KB, Free memory: '{free}'KB");

            var memoryStatus = new U()
            {
                Used = used,
                Free = free,
                DateTime = DateTime.Now
            };
            if (isRam)
            {
                (memoryStatus as RandomAccessMemoryStatus).DiskCache = int.Parse(groups["buffCache"].Value);
            }
            return memoryStatus;
        }
    }
}
