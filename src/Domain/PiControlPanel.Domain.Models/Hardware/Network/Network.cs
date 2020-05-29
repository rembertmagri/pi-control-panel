namespace PiControlPanel.Domain.Models.Hardware.Network
{
    using System.Collections.Generic;

    /// <summary>
    /// The network model.
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Gets or sets the network interfaces.
        /// </summary>
        public IList<NetworkInterface> NetworkInterfaces { get; set; }
    }
}
