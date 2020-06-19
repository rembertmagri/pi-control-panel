namespace PiControlPanel.Domain.Models.Hardware.Os
{
    /// <inheritdoc/>
    public class OsStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the system up time.
        /// </summary>
        public string Uptime { get; set; }
    }
}
