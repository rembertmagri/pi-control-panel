namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class FileSystem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the file system name.
        /// </summary>
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file system type.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the file system total capacity.
        /// </summary>
        [Required]
        public int Total { get; set; }
    }
}
