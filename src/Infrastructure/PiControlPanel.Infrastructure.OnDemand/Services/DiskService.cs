﻿namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Collections.Generic;
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

    /// <inheritdoc/>
    public class DiskService : BaseService<Disk>, IDiskService
    {
        private readonly ISubject<IList<FileSystemStatus>> fileSystemsStatusSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskService"/> class.
        /// </summary>
        /// <param name="fileSystemsStatusSubject">The file system status subject.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskService(ISubject<IList<FileSystemStatus>> fileSystemsStatusSubject, ILogger logger)
            : base(logger)
        {
            this.fileSystemsStatusSubject = fileSystemsStatusSubject;
        }

        /// <inheritdoc/>
        public Task<IList<FileSystemStatus>> GetFileSystemsStatusAsync(IList<string> fileSystemNames)
        {
            this.Logger.Debug("Infra layer -> DiskService -> GetFileSystemsStatusAsync");

            var result = BashCommands.Df.Bash();
            this.Logger.Trace($"Result of '{BashCommands.Df}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            IList<FileSystemStatus> fileSystemsStatus = new List<FileSystemStatus>();
            foreach (var fileSystemName in fileSystemNames)
            {
                var fileSystemInfo = lines.First(l => l.StartsWith($"{fileSystemName} "));
                var regex = new Regex(@"^(?<name>\S*)\s*(?<type>\S*)\s*(?<total>\S*)\s*(?<used>\S*)\s*(?<free>\S*).*$");
                var groups = regex.Match(fileSystemInfo).Groups;
                fileSystemsStatus.Add(new FileSystemStatus()
                {
                    FileSystemName = groups["name"].Value,
                    Used = int.Parse(groups["used"].Value),
                    Available = int.Parse(groups["free"].Value),
                    DateTime = DateTime.Now
                });
            }

            return Task.FromResult(fileSystemsStatus);
        }

        /// <inheritdoc/>
        public IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName)
        {
            this.Logger.Debug("Infra layer -> DiskService -> GetFileSystemStatusObservable");
            return this.fileSystemsStatusSubject
                .Select(l => l.FirstOrDefault(i => i.FileSystemName == fileSystemName))
                .AsObservable();
        }

        /// <inheritdoc/>
        public void PublishFileSystemsStatus(IList<FileSystemStatus> fileSystemsStatus)
        {
            this.Logger.Debug("Infra layer -> DiskService -> PublishFileSystemsStatus");
            this.fileSystemsStatusSubject.OnNext(fileSystemsStatus);
        }

        /// <inheritdoc/>
        protected override Disk GetModel()
        {
            var model = new Disk()
            {
                FileSystems = new List<FileSystem>()
            };

            var result = BashCommands.Df.Bash();
            this.Logger.Trace($"Result of '{BashCommands.Df}' command: '{result}'");
            string[] lines = result.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var fileSystemsInfo = lines.Where(l => l.StartsWith("/dev/") && !l.EndsWith("/boot"));
            var regex = new Regex(@"^(?<name>\S*)\s*(?<type>\S*)\s*(?<total>\S*)\s*(?<used>\S*)\s*(?<free>\S*).*$");

            foreach (var fileSystemInfo in fileSystemsInfo)
            {
                var groups = regex.Match(fileSystemInfo).Groups;
                model.FileSystems.Add(
                    new FileSystem()
                    {
                        Name = groups["name"].Value,
                        Type = groups["type"].Value,
                        Total = int.Parse(groups["total"].Value)
                    });
            }

            return model;
        }
    }
}
