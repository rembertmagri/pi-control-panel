namespace PiControlPanel.Domain.Models.Hardware.Disk
{
    /// <inheritdoc/>
    public class FileSystemStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the file system name.
        /// </summary>
        public string FileSystemName { get; set; }

        /// <summary>
        /// Gets or sets the space used.
        /// </summary>
        public int Used { get; set; }

        /// <summary>
        /// Gets or sets the space available.
        /// </summary>
        public int Available { get; set; }
    }
}
