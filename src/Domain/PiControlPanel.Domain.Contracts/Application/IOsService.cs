namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Application layer service for operations on Os model.
    /// </summary>
    public interface IOsService : IBaseService<Os>
    {
        /// <summary>
        /// Gets the most recent value of the operating system status.
        /// </summary>
        /// <returns>The OsStatus object.</returns>
        Task<OsStatus> GetLastStatusAsync();

        /// <summary>
        /// Gets the paged list of values for the operating system status.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the operating system status list.</returns>
        Task<PagingOutput<OsStatus>> GetStatusesAsync(PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the operating system status.
        /// </summary>
        /// <returns>The observable OsStatus.</returns>
        IObservable<OsStatus> GetStatusObservable();

        /// <summary>
        /// Retrieves and saves the operating system status.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveStatusAsync();
    }
}
