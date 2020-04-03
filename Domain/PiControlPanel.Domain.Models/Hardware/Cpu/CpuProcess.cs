namespace PiControlPanel.Domain.Models.Hardware.Cpu
{
    public class CpuProcess : BaseTimedObject
    {
        public int ProcessId { get; set; }

        public string User { get; set; }

        public string Priority { get; set; }

        public int NiceValue { get; set; }

        public int TotalMemory { get; set; }

        public int Ram { get; set; }

        public int SharedMemory { get; set; }

        public string State { get; set; }

        public double CpuPercentage { get; set; }

        public double RamPercentage { get; set; }

        public string TotalCpuTime { get; set; }

        public string Command { get; set; }
    }
}
