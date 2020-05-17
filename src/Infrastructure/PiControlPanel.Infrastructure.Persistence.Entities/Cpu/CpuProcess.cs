namespace PiControlPanel.Infrastructure.Persistence.Entities.Cpu
{
    using System.ComponentModel.DataAnnotations;

    public class CpuProcess : BaseTimedEntity
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int ProcessId { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        [Range(int.MinValue, int.MaxValue)]
        public int NiceValue { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TotalMemory { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Ram { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SharedMemory { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Range(0, 100)]
        public double CpuPercentage { get; set; }

        [Required]
        [Range(0, 100)]
        public double RamPercentage { get; set; }

        [Required]
        public string TotalCpuTime { get; set; }

        [Required]
        public string Command { get; set; }
    }
}
