namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    /// <inheritdoc/>
    public class CpuFrequency : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the CPU frequency in MHz.
        /// </summary>
        public int Frequency { get; set; }
    }
}
