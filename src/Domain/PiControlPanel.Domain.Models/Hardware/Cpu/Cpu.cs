namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    /// <summary>
    /// The CPU model.
    /// </summary>
    public class Cpu
    {
        /// <summary>
        /// Gets or sets the number of cores.
        /// </summary>
        public int Cores { get; set; }

        /// <summary>
        /// Gets or sets the CPU model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the maximum frequency.
        /// </summary>
        public int MaximumFrequency { get; set; }
    }
}
