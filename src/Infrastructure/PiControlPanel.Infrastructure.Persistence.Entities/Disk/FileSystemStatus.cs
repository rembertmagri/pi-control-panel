namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class FileSystemStatus : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the file system name.
        /// </summary>
        [Required]
        public string FileSystemName { get; set; }

        /// <summary>
        /// Gets or sets the space used.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Used { get; set; }

        /// <summary>
        /// Gets or sets the space available.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Available { get; set; }
    }
}
