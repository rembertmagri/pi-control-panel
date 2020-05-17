namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.ComponentModel.DataAnnotations;

    public class FileSystemStatus : BaseTimedEntity
    {
        [Required]
        public string FileSystemName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Used { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Available { get; set; }
    }
}
