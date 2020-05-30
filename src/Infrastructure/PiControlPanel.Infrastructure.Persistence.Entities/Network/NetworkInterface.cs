namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class NetworkInterface : BaseEntity
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        [Key]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the interface IP address.
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the interface subnet mask.
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 7)]
        public string SubnetMask { get; set; }

        /// <summary>
        /// Gets or sets the interface default gateway.
        /// </summary>
        public string DefaultGateway { get; set; }
    }
}
