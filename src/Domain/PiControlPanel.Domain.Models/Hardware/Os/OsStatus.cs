namespace PiControlPanel.Domain.Models.Hardware.Os
{
    /// <inheritdoc/>
    public class OsStatus : BaseTimedObject
    {
        /// <summary>
        /// Gets or sets the system up time.
        /// </summary>
        public string Uptime { get; set; }

        /// <summary>
        /// Gets or sets the number of upgradeable packages.
        /// </summary>
        public int UpgradeablePackages { get; set; }
    }
}
