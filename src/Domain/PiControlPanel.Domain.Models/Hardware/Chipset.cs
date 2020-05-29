namespace PiControlPanel.Domain.Models.Hardware
{
    /// <summary>
    /// The chipset model.
    /// </summary>
    public class Chipset
    {
        /// <summary>
        /// Gets or sets the chipset version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the chipset revision.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the chipset serial number.
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        /// Gets or sets the chipset model.
        /// </summary>
        public string Model { get; set; }
    }
}
