namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class Chipset : BaseEntity
    {
        /// <summary>
        /// Gets or sets the chipset serial number.
        /// </summary>
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Serial { get; set; }

        /// <summary>
        /// Gets or sets the chipset version.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the chipset revision.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the chipset model.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Model { get; set; }
    }
}
