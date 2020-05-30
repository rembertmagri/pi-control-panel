namespace PiControlPanel.Infrastructure.Persistence.Entities.Memory
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <inheritdoc/>
    public abstract class Memory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the memory capacity.
        /// </summary>
        [Required]
        public int Total { get; set; }
    }
}
