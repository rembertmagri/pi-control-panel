namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class Cpu : BaseEntity
    {
        /// <summary>
        /// Gets or sets the CPU model.
        /// </summary>
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the number of cores.
        /// </summary>
        [Required]
        public int Cores { get; set; }

        /// <summary>
        /// Gets or sets the CPU maximum frequency.
        /// </summary>
        [Required]
        public int MaximumFrequency { get; set; }
    }
}
