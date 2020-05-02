namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.ComponentModel.DataAnnotations;

    public class Interface : BaseEntity
    {
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string IpAddress { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 7)]
        public string SubnetMask { get; set; }

        public string DefaultGateway { get; set; }
    }
}
