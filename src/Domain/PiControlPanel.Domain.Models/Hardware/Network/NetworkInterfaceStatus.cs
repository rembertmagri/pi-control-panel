namespace PiControlPanel.Domain.Models.Hardware.Network
{
    /// <inheritdoc/>
    public class NetworkInterfaceStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        public string NetworkInterfaceName { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes received.
        /// </summary>
        public long TotalReceived { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes seny.
        /// </summary>
        public long TotalSent { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes received per second.
        /// </summary>
        public double ReceiveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes sent per second.
        /// </summary>
        public double SendSpeed { get; set; }
    }
}
