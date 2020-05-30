namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// A class representing a database entity with identifier and time information.
    /// </summary>
    public abstract class BaseTimedEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the entity datetime.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }
    }
}
