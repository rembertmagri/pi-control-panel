namespace PiControlPanel.Infrastructure.Persistence.Entities.Memory
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class RandomAccessMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Gets or sets the space used for disk cache.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int DiskCache { get; set; }
    }
}
