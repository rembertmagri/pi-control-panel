namespace PiControlPanel.Domain.Models.Hardware.Memory
{
    /// <inheritdoc/>
    public class RandomAccessMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Gets or sets the space used for disk cache.
        /// </summary>
        public int DiskCache { get; set; }
    }
}
