namespace PiControlPanel.Infrastructure.Persistence.Entities.Disk
{
    using System.ComponentModel.DataAnnotations;

    public class FileSystem : BaseEntity
    {
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        public int Total { get; set; }
    }
}
