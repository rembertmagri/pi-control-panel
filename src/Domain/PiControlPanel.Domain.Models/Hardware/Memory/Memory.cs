namespace PiControlPanel.Domain.Models.Hardware.Memory
{
    /// <summary>
    /// The memory model.
    /// </summary>
    public abstract class Memory
    {
        /// <summary>
        /// Gets or sets the memory capacity.
        /// </summary>
        public int Total { get; set; }
    }
}
