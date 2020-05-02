namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.ComponentModel.DataAnnotations;

    public class NetworkInterfaceStatus : BaseTimedEntity
    {
        [Required]
        public string NetworkInterfaceName { get; set; }
        
        [Required]
        public long TotalReceived { get; set; }

        [Required]
        public long TotalSent { get; set; }

        [Required]
        public long ReceiveSpeed { get; set; }

        [Required]
        public long SendSpeed { get; set; }
    }
}
