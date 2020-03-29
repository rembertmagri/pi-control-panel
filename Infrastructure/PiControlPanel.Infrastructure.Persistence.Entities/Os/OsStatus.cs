namespace PiControlPanel.Infrastructure.Persistence.Entities.Os
{
    using System.ComponentModel.DataAnnotations;

    public class OsStatus : BaseTimedEntity
    {
        [Required]
        [StringLength(50)]
        public string Uptime { get; set; }
    }
}
