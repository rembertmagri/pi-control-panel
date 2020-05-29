namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    using System.Collections.Generic;

    /// <inheritdoc/>
    public class CpuLoadStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the CPU last minute average load.
        /// </summary>
        public double LastMinuteAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU last 5 minutes average load.
        /// </summary>
        public double Last5MinutesAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU last 15 minutes average load.
        /// </summary>
        public double Last15MinutesAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU kernel load.
        /// </summary>
        public double KernelRealTime { get; set; }

        /// <summary>
        /// Gets or sets the CPU user load.
        /// </summary>
        public double UserRealTime { get; set; }

        /// <summary>
        /// Gets or sets the list of running processes.
        /// </summary>
        public IList<CpuProcess> Processes { get; set; }
    }
}
