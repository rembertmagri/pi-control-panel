namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <inheritdoc/>
    public class Disk : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the disk file systems.
        /// </summary>
        public ICollection<FileSystem> FileSystems { get; set; }
    }
}
