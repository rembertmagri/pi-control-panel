namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    /// <inheritdoc/>
    public class CpuSensorsStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the CPU temperature in °C.
        /// </summary>
        public double Temperature { get; set; }
    }
}
