namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class CpuFrequency : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the CPU frequency.
        /// </summary>
        [Required]
        [Range(0, 10000)]
        public int Frequency { get; set; }
    }
}
