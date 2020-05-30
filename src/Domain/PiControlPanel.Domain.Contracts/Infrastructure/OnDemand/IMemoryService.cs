namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Infrastructure layer service for on demand operations on Memory model.
    /// </summary>
    /// /// <typeparam name="TMemory">The Memory generic type parameter.</typeparam>
    /// <typeparam name="TMemoryStatus">The MemoryStatus generic type parameter.</typeparam>
    public interface IMemoryService<TMemory, TMemoryStatus> : IBaseService<TMemory>
        where TMemory : Memory
        where TMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Gets the value of the memory status.
        /// </summary>
        /// <returns>The MemoryStatus object.</returns>
        Task<TMemoryStatus> GetStatusAsync();

        /// <summary>
        /// Gets an observable of the memory status.
        /// </summary>
        /// <returns>The observable MemoryStatus.</returns>
        IObservable<TMemoryStatus> GetStatusObservable();

        /// <summary>
        /// Publishes the value of the memory status.
        /// </summary>
        /// <param name="status">The value to be publlished.</param>
        void PublishStatus(TMemoryStatus status);
    }
}
