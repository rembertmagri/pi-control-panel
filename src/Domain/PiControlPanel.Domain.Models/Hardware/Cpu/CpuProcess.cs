namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    /// <inheritdoc/>
    public class CpuProcess : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process owner.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the process priority.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the process nice value.
        /// </summary>
        public int NiceValue { get; set; }

        /// <summary>
        /// Gets or sets the process total used memory.
        /// </summary>
        public string TotalMemory { get; set; }

        /// <summary>
        /// Gets or sets the process used RAM.
        /// </summary>
        public string Ram { get; set; }

        /// <summary>
        /// Gets or sets the process used shared memory.
        /// </summary>
        public string SharedMemory { get; set; }

        /// <summary>
        /// Gets or sets the process state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the process CPU usage.
        /// </summary>
        public double CpuPercentage { get; set; }

        /// <summary>
        /// Gets or sets the process RAM usage.
        /// </summary>
        public double RamPercentage { get; set; }

        /// <summary>
        /// Gets or sets the process total CPU time.
        /// </summary>
        public string TotalCpuTime { get; set; }

        /// <summary>
        /// Gets or sets the process command.
        /// </summary>
        public string Command { get; set; }
    }
}
