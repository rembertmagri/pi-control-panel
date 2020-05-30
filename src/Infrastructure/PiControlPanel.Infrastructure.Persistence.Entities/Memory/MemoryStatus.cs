namespace PiControlPanel.Infrastructure.Persistence.Entities.Memory
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public abstract class MemoryStatus : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the total used memory.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Used { get; set; }

        /// <summary>
        /// Gets or sets the free memory.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Free { get; set; }
    }
}
