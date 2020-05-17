namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    public class Cpu : BaseEntity
    {
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Model { get; set; }

        [Required]
        public int Cores { get; set; }

        [Required]
        public int MaximumFrequency { get; set; }
    }
}
