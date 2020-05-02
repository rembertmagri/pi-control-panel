namespace PiControlPanel.Domain.Models.Hardware.Network
{
    public class NetworkInterfaceStatus : BaseTimedObject
    {
        public string NetworkInterfaceName { get; set; }

        public long TotalReceived { get; set; }

        public long TotalSent { get; set; }

        public double ReceiveSpeed { get; set; }

        public double SendSpeed { get; set; }
    }
}
