namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Application layer service for operations on CPU model.
    /// </summary>
    public interface ICpuService : IBaseService<Cpu>
    {
        /// <summary>
        /// Gets the most recent value of the CPU load status.
        /// </summary>
        /// <returns>The CpuLoadStatus object.</returns>
        Task<CpuLoadStatus> GetLastLoadStatusAsync();

        /// <summary>
        /// Gets the paged list of values for the CPU load status.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the CPU load status list.</returns>
        Task<PagingOutput<CpuLoadStatus>> GetLoadStatusesAsync(PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the CPU load status.
        /// </summary>
        /// <returns>The observable CpuLoadStatus.</returns>
        IObservable<CpuLoadStatus> GetLoadStatusObservable();

        /// <summary>
        /// Gets a dictionary of real time CPU load values.
        /// </summary>
        /// <param name="dateTimes">The list of datetime to be retrieved.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The values of real time CPU load values.</returns>
        Task<IDictionary<DateTime, double>> GetTotalRealTimeLoadsAsync(
            IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the most recent value of the CPU temperature.
        /// </summary>
        /// <returns>The CpuTemperature object.</returns>
        Task<CpuTemperature> GetLastTemperatureAsync();

        /// <summary>
        /// Gets the paged list of values for the CPU temperature.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the CPU temperature list.</returns>
        Task<PagingOutput<CpuTemperature>> GetTemperaturesAsync(PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the CPU temperature.
        /// </summary>
        /// <returns>The observable CpuTemperature.</returns>
        IObservable<CpuTemperature> GetTemperatureObservable();

        /// <summary>
        /// Gets the most recent value of the CPU frequency.
        /// </summary>
        /// <returns>The CpuFrequency object.</returns>
        Task<CpuFrequency> GetLastFrequencyAsync();

        /// <summary>
        /// Gets the paged list of values for the CPU frequency.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the CPU frequency list.</returns>
        Task<PagingOutput<CpuFrequency>> GetFrequenciesAsync(PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the CPU frequency.
        /// </summary>
        /// <returns>The observable CpuFrequency.</returns>
        IObservable<CpuFrequency> GetFrequencyObservable();

        /// <summary>
        /// Retrieves and saves the CPU frequency.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveLoadStatusAsync();

        /// <summary>
        /// Retrieves and saves the CPU temperature.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveTemperatureAsync();

        /// <summary>
        /// Retrieves and saves the CPU frequency.
        /// </summary>
        /// <param name="samplingInterval">The sampling interval in milliseconds to be used to calculate the frequency.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveFrequencyAsync(int samplingInterval);
    }
}
