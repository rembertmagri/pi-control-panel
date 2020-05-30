namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <summary>
    /// Infrastructure layer service for on demand operations on Disk model.
    /// </summary>
    public interface IDiskService : IBaseService<Disk>
    {
        /// <summary>
        /// Gets the value of the file system status for each file system.
        /// </summary>
        /// <param name="fileSystemNames">The list of file systems.</param>
        /// <returns>A list of FileSystemStatus objects.</returns>
        Task<IList<FileSystemStatus>> GetFileSystemsStatusAsync(IList<string> fileSystemNames);

        /// <summary>
        /// Gets an observable of the file system status.
        /// </summary>
        /// <param name="fileSystemName">The file system name.</param>
        /// <returns>The observable FileSystemStatus.</returns>
        IObservable<FileSystemStatus> GetFileSystemStatusObservable(string fileSystemName);

        /// <summary>
        /// Publishes the value of the file systems status.
        /// </summary>
        /// <param name="fileSystemsStatus">The values to be publlished.</param>
        void PublishFileSystemsStatus(IList<FileSystemStatus> fileSystemsStatus);
    }
}
