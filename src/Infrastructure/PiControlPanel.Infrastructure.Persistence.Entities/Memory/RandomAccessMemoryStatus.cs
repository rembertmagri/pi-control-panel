namespace PiControlPanel.Infrastructure.Persistence.Entities.Memory
{
    using System.ComponentModel.DataAnnotations;

    public class RandomAccessMemoryStatus : MemoryStatus
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int DiskCache { get; set; }
    }
}
