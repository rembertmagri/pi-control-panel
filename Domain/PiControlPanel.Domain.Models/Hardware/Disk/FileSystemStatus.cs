namespace PiControlPanel.Domain.Models.Hardware.Disk
{
    public class FileSystemStatus : BaseTimedObject
    {
        public string FileSystemName { get; set; }

        public int Used { get; set; }

        public int Available { get; set; }
    }
}
