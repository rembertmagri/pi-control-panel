namespace PiControlPanel.Domain.Models.Hardware.Network
{
    /// <summary>
    /// The network interface model.
    /// </summary>
    public class NetworkInterface
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the interface IP address.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the interface subnet mask.
        /// </summary>
        public string SubnetMask { get; set; }

        /// <summary>
        /// Gets or sets the interface default gateway.
        /// </summary>
        public string DefaultGateway { get; set; }
    }
}
