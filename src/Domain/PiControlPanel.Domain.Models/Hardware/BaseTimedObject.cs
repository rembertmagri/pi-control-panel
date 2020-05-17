namespace PiControlPanel.Domain.Models.Hardware
{
    using System;

    public class BaseTimedObject
    {
        public Guid ID { get; set; }

        public DateTime DateTime { get; set; }
    }
}
