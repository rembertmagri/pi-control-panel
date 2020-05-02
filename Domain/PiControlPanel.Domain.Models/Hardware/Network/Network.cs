namespace PiControlPanel.Domain.Models.Hardware.Network
{
    using System.Collections.Generic;

    public class Network
    {
        public IList<Interface> Interfaces { get; set; }
    }
}
