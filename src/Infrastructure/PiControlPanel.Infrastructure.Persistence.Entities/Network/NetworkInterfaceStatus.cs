namespace PiControlPanel.Infrastructure.Persistence.Entities.Network
{
    using System.ComponentModel.DataAnnotations;

    /// <inheritdoc/>
    public class NetworkInterfaceStatus : BaseTimedEntity
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        [Required]
        public string NetworkInterfaceName { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes received.
        /// </summary>
        [Required]
        public long TotalReceived { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes sent.
        /// </summary>
        [Required]
        public long TotalSent { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes received per second.
        /// </summary>
        [Required]
        public double ReceiveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes sent per second.
        /// </summary>
        [Required]
        public double SendSpeed { get; set; }
    }
}
