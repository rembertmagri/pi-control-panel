namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public interface IDiskService : IBaseService<Disk>
    {
        Task<IList<FileSystemStatus>> GetFileSystemsStatusAsync(IList<string> fileSystemNames);

        IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName);

        void PublishFileSystemsStatus(IList<FileSystemStatus> fileSystemsStatus);
    }
}
