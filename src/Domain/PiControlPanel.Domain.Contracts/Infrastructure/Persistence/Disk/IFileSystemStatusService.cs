namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;

    public interface IFileSystemStatusService
    {
        Task<FileSystemStatus> GetLastAsync(string fileSystemName);

        Task<IEnumerable<FileSystemStatus>> GetAllAsync(string fileSystemName);

        Task<PagingOutput<FileSystemStatus>> GetPageAsync(string fileSystemName, PagingInput pagingInput);

        Task AddManyAsync(IEnumerable<FileSystemStatus> fileSystemsStatus);
    }
}
