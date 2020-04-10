namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    public class CpuFrequency : BaseTimedEntity
    {
        [Required]
        [Range(0, 10000)]
        public int Frequency { get; set; }
    }
}
