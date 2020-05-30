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

    /// <inheritdoc/>
    public class MemoryService<TMemory, TMemoryStatus> : BaseService<TMemory>, IMemoryService<TMemory, TMemoryStatus>
        where TMemory : Memory, new()
        where TMemoryStatus : MemoryStatus, new()
    {
        private readonly ISubject<TMemoryStatus> memoryStatusSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryService{T, U}"/> class.
        /// </summary>
        /// <param name="memoryStatusSubject">The memory status subject.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public MemoryService(ISubject<TMemoryStatus> memoryStatusSubject, ILogger logger)
            : base(logger)
        {
            this.memoryStatusSubject = memoryStatusSubject;
        }

        /// <inheritdoc/>
        public Task<TMemoryStatus> GetStatusAsync()
        {
            this.logger.Debug("Infra layer -> MemoryService -> GetStatusAsync");
            var memoryStatus = this.GetMemoryStatus();
            return Task.FromResult(memoryStatus);
        }

        /// <inheritdoc/>
        public IObservable<TMemoryStatus> GetStatusObservable()
        {
            this.logger.Debug("Infra layer -> MemoryService -> GetStatusObservable");
            return this.memoryStatusSubject.AsObservable();
        }

        /// <inheritdoc/>
        public void PublishStatus(TMemoryStatus status)
        {
            this.logger.Debug("Infra layer -> MemoryService -> PublishStatus");
            this.memoryStatusSubject.OnNext(status);
        }

        /// <inheritdoc/>
        protected override TMemory GetModel()
        {
            var result = BashCommands.Free.Bash();
            this.logger.Trace($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var memoryTypeName = typeof(TMemoryStatus) == typeof(RandomAccessMemory) ? "Mem:" : "Swap:";
            var memoryInfo = lines.First(l => l.StartsWith(memoryTypeName));
            var regex = new Regex(@"^\w*:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*(?<shared>\d*)\s*(?<buffcache>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var total = int.Parse(groups["total"].Value);
            this.logger.Trace($"Total memory: '{total}'KB");

            return new TMemory()
            {
                Total = total
            };
        }

        private TMemoryStatus GetMemoryStatus()
        {
            var result = BashCommands.Free.Bash();
            this.logger.Trace($"Result of '{BashCommands.Free}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var isRam = typeof(TMemoryStatus) == typeof(RandomAccessMemory);
            var memoryTypeName = isRam ? "Mem:" : "Swap:";
            var memoryInfo = lines.First(l => l.StartsWith(memoryTypeName));
            var regex = isRam ?
                new Regex(@"^Mem:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*(?<shared>\d*)\s*(?<buffCache>\d*)\s*.*$") :
                new Regex(@"^Swap:\s*(?<total>\d*)\s*(?<used>\d*)\s*(?<free>\d*)\s*.*$");
            var groups = regex.Match(memoryInfo).Groups;
            var used = int.Parse(groups["used"].Value);
            var free = int.Parse(groups["free"].Value);
            this.logger.Trace($"Used memory: '{used}'KB, Free memory: '{free}'KB");

            var memoryStatus = new TMemoryStatus()
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
