namespace PiControlPanel.Domain.Models.Hardware.Disk
{
    using System.Collections.Generic;

    public class Disk
    {
        public IList<FileSystem> FileSystems { get; set; }
    }
}
