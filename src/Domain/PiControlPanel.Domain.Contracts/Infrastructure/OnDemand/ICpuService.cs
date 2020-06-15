namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// Infrastructure layer service for on demand operations on CPU model.
    /// </summary>
    public interface ICpuService : IBaseService<Cpu>
    {
        /// <summary>
        /// Gets the value of the CPU load status.
        /// </summary>
        /// <returns>The CpuLoadStatus object.</returns>
        Task<CpuLoadStatus> GetLoadStatusAsync();

        /// <summary>
        /// Gets an observable of the CPU load status.
        /// </summary>
        /// <returns>The observable CpuLoadStatus.</returns>
        IObservable<CpuLoadStatus> GetLoadStatusObservable();

        /// <summary>
        /// Publishes the value of the CPU load status.
        /// </summary>
        /// <param name="loadStatus">The value to be publlished.</param>
        void PublishLoadStatus(CpuLoadStatus loadStatus);

        /// <summary>
        /// Gets the value of the CPU sensors status.
        /// </summary>
        /// <returns>The CpuSensorsStatus object.</returns>
        Task<CpuSensorsStatus> GetSensorsStatusAsync();

        /// <summary>
        /// Gets an observable of the CPU sensors status.
        /// </summary>
        /// <returns>The observable CpuSensorsStatus.</returns>
        IObservable<CpuSensorsStatus> GetSensorsStatusObservable();

        /// <summary>
        /// Publishes the value of the CPU sensors status.
        /// </summary>
        /// <param name="sensorsStatus">The value to be publlished.</param>
        void PublishSensorsStatus(CpuSensorsStatus sensorsStatus);

        /// <summary>
        /// Gets the value of the CPU frequency.
        /// </summary>
        /// <param name="samplingInterval">The sampling interval in milliseconds to be used to calculate the frequency.</param>
        /// <returns>The CpuFrequency object.</returns>
        Task<CpuFrequency> GetFrequencyAsync(int samplingInterval);

        /// <summary>
        /// Gets an observable of the CPU frequency.
        /// </summary>
        /// <returns>The observable CpuFrequency.</returns>
        IObservable<CpuFrequency> GetFrequencyObservable();

        /// <summary>
        /// Publishes the value of the CPU frequency.
        /// </summary>
        /// <param name="frequency">The value to be publlished.</param>
        void PublishFrequency(CpuFrequency frequency);
    }
}
