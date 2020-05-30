namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class CpuProcess : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process owner.
        /// </summary>
        [Required]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the process priority.
        /// </summary>
        [Required]
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the process nice value.
        /// </summary>
        [Required]
        [Range(int.MinValue, int.MaxValue)]
        public int NiceValue { get; set; }

        /// <summary>
        /// Gets or sets the process total used memory.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int TotalMemory { get; set; }

        /// <summary>
        /// Gets or sets the process used RAM.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Ram { get; set; }

        /// <summary>
        /// Gets or sets the process used shared memory.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int SharedMemory { get; set; }

        /// <summary>
        /// Gets or sets the process state.
        /// </summary>
        [Required]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the process CPU usage.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public double CpuPercentage { get; set; }

        /// <summary>
        /// Gets or sets the process RAM usage.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public double RamPercentage { get; set; }

        /// <summary>
        /// Gets or sets the process total CPU time.
        /// </summary>
        [Required]
        public string TotalCpuTime { get; set; }

        /// <summary>
        /// Gets or sets the process command.
        /// </summary>
        [Required]
        public string Command { get; set; }
    }
}
