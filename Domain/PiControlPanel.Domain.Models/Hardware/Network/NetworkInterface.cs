namespace PiControlPanel.Domain.Models.Hardware.Network
{
    public class NetworkInterface
    {
        public string Name { get; set; }

        public string IpAddress { get; set; }

        public string SubnetMask { get; set; }

        public string DefaultGateway { get; set; }
    }
}
