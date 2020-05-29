namespace PiControlPanel.Domain.Models.Hardware
{
    /// <summary>
    /// The GPU model.
    /// </summary>
    public class Gpu
    {
        /// <summary>
        /// Gets or sets the GPU total memory.
        /// </summary>
        public int Memory { get; set; }

        /// <summary>
        /// Gets or sets the GPU frequency.
        /// </summary>
        public int Frequency { get; set; }
    }
}
