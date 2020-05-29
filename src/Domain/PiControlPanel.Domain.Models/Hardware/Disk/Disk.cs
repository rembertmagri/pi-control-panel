namespace PiControlPanel.Domain.Models.Hardware.Disk
{
    using System.Collections.Generic;

    /// <summary>
    /// The disk model.
    /// </summary>
    public class Disk
    {
        /// <summary>
        /// Gets or sets the disk file systems.
        /// </summary>
        public IList<FileSystem> FileSystems { get; set; }
    }
}
