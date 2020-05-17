namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    using System.Collections.Generic;

    public class CpuLoadStatus : BaseTimedObject
    {
        public double LastMinuteAverage { get; set; }

        public double Last5MinutesAverage { get; set; }

        public double Last15MinutesAverage { get; set; }

        public double KernelRealTime { get; set; }

        public double UserRealTime { get; set; }

        public IList<CpuProcess> Processes { get; set; }
    }
}
