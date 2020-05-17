namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Gpu : BaseEntity
    {
        [Key]
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        public int Memory { get; set; }

        [Required]
        public int Frequency { get; set; }
    }
}
