namespace PiControlPanel.Infrastructure.Persistence.Entities.Os
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Os : BaseEntity
    {
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Kernel { get; set; }

        [Required]
        [StringLength(50)]
        public string Hostname { get; set; }
    }
}
