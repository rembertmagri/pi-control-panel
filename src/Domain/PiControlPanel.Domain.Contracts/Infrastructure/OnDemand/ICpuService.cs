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
        /// Gets the value of the CPU temperature.
        /// </summary>
        /// <returns>The CpuTemperature object.</returns>
        Task<CpuTemperature> GetTemperatureAsync();

        /// <summary>
        /// Gets an observable of the CPU temperature.
        /// </summary>
        /// <returns>The observable CpuTemperature.</returns>
        IObservable<CpuTemperature> GetTemperatureObservable();

        /// <summary>
        /// Publishes the value of the CPU temperature.
        /// </summary>
        /// <param name="temperature">The value to be publlished.</param>
        void PublishTemperature(CpuTemperature temperature);

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
