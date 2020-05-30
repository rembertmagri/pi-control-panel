namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// Infrastructure layer service for persistence operations on CPU load status model.
    /// </summary>
    public interface ICpuLoadStatusService : IBaseTimedObjectService<CpuLoadStatus>
    {
        /// <summary>
        /// Gets the CPU load status values for the given datetimes.
        /// </summary>
        /// <param name="dateTimes">The datetimes to be considered.</param>
        /// <returns>A <see cref="Task{IDictionary}"/> representing the result of the asynchronous operation.</returns>
        Task<IDictionary<DateTime, CpuLoadStatus>> GetCpuLoadStatusesAsync(
            IEnumerable<DateTime> dateTimes);
    }
}
