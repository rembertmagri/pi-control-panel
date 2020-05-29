namespace PiControlPanel.Domain.Models.Enums
{
    /// <summary>
    /// The enum representation of the CPU max frequency levels.
    /// </summary>
    public enum CpuMaxFrequencyLevel
    {
        /// <summary>
        /// 1500 MHz (Default)
        /// </summary>
        Default = 1500,

        /// <summary>
        /// 1750 MHz
        /// </summary>
        High = 1750,

        /// <summary>
        /// 2000 MHz
        /// </summary>
        Maximum = 2000
    }
}
