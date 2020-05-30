namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class CpuLoadStatus : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the CPU last minute load.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double LastMinuteAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU last 5 minutes load.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double Last5MinutesAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU last 15 minutes load.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double Last15MinutesAverage { get; set; }

        /// <summary>
        /// Gets or sets the CPU real time kernel load.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public double KernelRealTime { get; set; }

        /// <summary>
        /// Gets or sets the CPU real time user load.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public double UserRealTime { get; set; }

        /// <summary>
        /// Gets or sets the CPU list of running processes.
        /// </summary>
        public ICollection<CpuProcess> CpuProcesses { get; set; }
    }
}
