namespace PiControlPanel.Domain.Models.Hardware.Os
{
    /// <summary>
    /// The operating system model.
    /// </summary>
    public class Os
    {
        /// <summary>
        /// Gets or sets the operating system name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the operating system kernel name.
        /// </summary>
        public string Kernel { get; set; }

        /// <summary>
        /// Gets or sets the system hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SSH server is running.
        /// </summary>
        public bool SshStarted { get; set; }

        /// <summary>
        /// Gets or sets the SSH port.
        /// </summary>
        public int SshPort { get; set; }

        /// <summary>
        /// Gets or sets the number of upgradeable packages.
        /// </summary>
        public int UpgradeablePackages { get; set; }
    }
}
