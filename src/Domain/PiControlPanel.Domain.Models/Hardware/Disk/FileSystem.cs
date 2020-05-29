namespace PiControlPanel.Domain.Models.Hardware.Disk
{
    /// <summary>
    /// The disk file system model.
    /// </summary>
    public class FileSystem
    {
        /// <summary>
        /// Gets or sets the file system name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file system type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the file system total capacity.
        /// </summary>
        public int Total { get; set; }
    }
}
