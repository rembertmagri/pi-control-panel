namespace PiControlPanel.Infrastructure.Persistence.Entities.Os
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class OsStatus : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the system up time.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Uptime { get; set; }

        /// <summary>
        /// Gets or sets the number of upgradeable packages.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int UpgradeablePackages { get; set; }
    }
}
