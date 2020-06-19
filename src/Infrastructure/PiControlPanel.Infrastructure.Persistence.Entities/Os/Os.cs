namespace PiControlPanel.Infrastructure.Persistence.Entities.Os
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <inheritdoc/>
    public class Os : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the operating system name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the operating system kernel name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Kernel { get; set; }

        /// <summary>
        /// Gets or sets the system hostname.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the number of upgradeable packages.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int UpgradeablePackages { get; set; }
    }
}
