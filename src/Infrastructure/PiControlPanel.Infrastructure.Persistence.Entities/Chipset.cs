namespace PiControlPanel.Infrastructure.Persistence.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Chipset : BaseEntity
    {
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Serial { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; }

        [Required]
        [StringLength(50)]
        public string Revision { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }
    }
}
