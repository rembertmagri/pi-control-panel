namespace PiControlPanel.Domain.Models.Hardware.Memory
{
    public abstract class MemoryStatus : BaseTimedObject
    {
        public int Used { get; set; }

        public int Free { get; set; }
    }
}
