namespace PiControlPanel.Domain.Models.Hardware.Memory
{
    /// <inheritdoc/>
    public abstract class MemoryStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the total used memory.
        /// </summary>
        public int Used { get; set; }

        /// <summary>
        /// Gets or sets the free memory.
        /// </summary>
        public int Free { get; set; }
    }
}
