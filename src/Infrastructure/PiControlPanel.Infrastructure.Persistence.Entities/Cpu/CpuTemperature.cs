namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class CpuTemperature : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the CPU temperature.
        /// </summary>
        [Required]
        [Range(-273, 473)]
        public int Temperature { get; set; }
    }
}
