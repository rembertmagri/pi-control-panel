namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Application layer service for operations on Disk model.
    /// </summary>
    public interface IDiskService : IBaseService<Disk>
    {
        /// <summary>
        /// Gets the most recent value of the file system status.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <returns>The FileSystemStatus object.</returns>
        Task<FileSystemStatus> GetLastFileSystemStatusAsync(string fileSystemName);

        /// <summary>
        /// Gets the paged list of values for the file system status.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the file system status list.</returns>
        Task<PagingOutput<FileSystemStatus>> GetFileSystemStatusesAsync(string fileSystemName, PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the file system status.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <returns>The observable FileSystemStatus.</returns>
        IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName);

        /// <summary>
        /// Retrieves and saves the file system status.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveFileSystemStatusAsync();
    }
}
