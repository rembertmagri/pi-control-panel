namespace PiControlPanel.Infrastructure.Persistence.Entities.Memory
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Memory : BaseEntity
    {
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        public int Total { get; set; }
    }
}
