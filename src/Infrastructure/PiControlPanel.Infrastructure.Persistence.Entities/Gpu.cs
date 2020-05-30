namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <inheritdoc/>
    public class Gpu : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the GPU total memory.
        /// </summary>
        [Required]
        public int Memory { get; set; }

        /// <summary>
        /// Gets or sets the GPU frequency.
        /// </summary>
        [Required]
        public int Frequency { get; set; }
    }
}
