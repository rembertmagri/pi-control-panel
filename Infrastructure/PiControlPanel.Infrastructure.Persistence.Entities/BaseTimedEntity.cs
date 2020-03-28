namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class BaseTimedEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }
    }
}
