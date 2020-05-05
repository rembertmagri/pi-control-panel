namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;

    public interface IDiskService : IBaseService<Disk>
    {
        Task<FileSystemStatus> GetLastFileSystemStatusAsync(string fileSystemName);

        Task<PagingOutput<FileSystemStatus>> GetFileSystemStatusesAsync(string fileSystemName, PagingInput pagingInput);

        IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName);

        Task SaveFileSystemStatusAsync();

    }
}
