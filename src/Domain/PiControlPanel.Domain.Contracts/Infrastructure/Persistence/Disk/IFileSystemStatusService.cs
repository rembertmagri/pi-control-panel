namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Infrastructure layer service for persistence operations on disk file system model.
    /// </summary>
    public interface IFileSystemStatusService
    {
        /// <summary>
        /// Gets the most recent value for a specific file system.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <returns>A <see cref="Task{FileSystemStatus}"/> representing the result of the asynchronous operation.</returns>
        Task<FileSystemStatus> GetLastAsync(string fileSystemName);

        /// <summary>
        /// Gets all status of a file system.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <returns>A <see cref="Task{FileSystemStatus}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<FileSystemStatus>> GetAllAsync(string fileSystemName);

        /// <summary>
        /// Gets the paged status of a file system.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>A <see cref="Task{PagingOutput}"/> representing the result of the asynchronous operation.</returns>
        Task<PagingOutput<FileSystemStatus>> GetPageAsync(string fileSystemName, PagingInput pagingInput);

        /// <summary>
        /// Persists multiple values in the database.
        /// </summary>
        /// <param name="fileSystemsStatus">The file system status list to be created.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddManyAsync(IEnumerable<FileSystemStatus> fileSystemsStatus);
    }
}
