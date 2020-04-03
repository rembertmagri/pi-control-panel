namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CpuLoadStatus : BaseTimedEntity
    {
        [Required]
        [Range(0, double.MaxValue)]
        public double LastMinuteAverage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Last5MinutesAverage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Last15MinutesAverage { get; set; }

        [Required]
        [Range(0, 100)]
        public double KernelRealTime { get; set; }

        [Required]
        [Range(0, 100)]
        public double UserRealTime { get; set; }

        public ICollection<CpuProcess> CpuProcesses { get; set; }
    }
}
