namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    public class CpuTemperature : BaseTimedEntity
    {
        [Required]
        [Range(-273, 473)]
        public int Temperature { get; set; }
    }
}
