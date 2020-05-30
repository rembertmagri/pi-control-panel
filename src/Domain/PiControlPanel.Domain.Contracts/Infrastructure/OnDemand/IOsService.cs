namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Os;

    /// <summary>
    /// Infrastructure layer service for on demand operations on operating system model.
    /// </summary>
    public interface IOsService : IBaseService<Os>
    {
        /// <summary>
        /// Gets the value of the operating system status.
        /// </summary>
        /// <returns>The OsStatus object.</returns>
        Task<OsStatus> GetStatusAsync();

        /// <summary>
        /// Gets an observable of the operating system status.
        /// </summary>
        /// <returns>The observable OsStatus.</returns>
        IObservable<OsStatus> GetStatusObservable();

        /// <summary>
        /// Publishes the value of the operating system status.
        /// </summary>
        /// <param name="status">The value to be publlished.</param>
        void PublishStatus(OsStatus status);
    }
}
