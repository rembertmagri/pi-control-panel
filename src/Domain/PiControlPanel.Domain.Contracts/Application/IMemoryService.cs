namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Application layer service for operations on Memory model.
    /// </summary>
    /// <typeparam name="TMemory">The Memory generic type parameter.</typeparam>
    /// <typeparam name="TMemoryStatus">The MemoryStatus generic type parameter.</typeparam>
    public interface IMemoryService<TMemory, TMemoryStatus> : IBaseService<TMemory>
        where TMemory : Memory
        where TMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Gets the most recent value of the memory status.
        /// </summary>
        /// <returns>The MemoryStatus object.</returns>
        Task<TMemoryStatus> GetLastStatusAsync();

        /// <summary>
        /// Gets the paged list of values for the memory status.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the memory status list.</returns>
        Task<PagingOutput<TMemoryStatus>> GetStatusesAsync(PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the memory status.
        /// </summary>
        /// <returns>The observable MemoryStatus.</returns>
        IObservable<TMemoryStatus> GetStatusObservable();

        /// <summary>
        /// Retrieves and saves the memory status.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveStatusAsync();
    }
}
