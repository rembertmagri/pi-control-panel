namespace PiControlPanel.Domain.Contracts.Constants
{
    /// <summary>
    /// Contains the bash commands used to retrieve information from Raspberry Pi.
    /// </summary>
    public class BashCommands
    {
        /// <summary>
        /// Displays the CPU temperature.
        /// </summary>
        public const string MeasureTemp = "vcgencmd measure_temp";

        /// <summary>
        /// Displays the CPU voltage.
        /// </summary>
        public const string MeasureVolts = "vcgencmd measure_volts";

        /// <summary>
        /// Displays the CPU throttled state.
        /// </summary>
        public const string GetThrottled = "vcgencmd get_throttled";

        /// <summary>
        /// Concatenates time_in_state file to standard output.
        /// </summary>
        public const string CatCpuFreqStats = "cat /sys/devices/system/cpu/cpu0/cpufreq/stats/time_in_state";

        /// <summary>
        /// Reboots the system, equivalent to reboot.
        /// </summary>
        public const string SudoReboot = "sudo shutdown -r now";

        /// <summary>
        /// Shutdown linux.
        /// </summary>
        public const string SudoShutdown = "sudo shutdown -h now";

        /// <summary>
        /// Concatenates shadow file to standard output.
        /// </summary>
        public const string SudoCatEtcShadow = "sudo cat /etc/shadow | grep {0}";

        /// <summary>
        /// Toolkit implementing SSL and TSL.
        /// </summary>
        public const string OpenSslPasswd = "openssl passwd -{0} -salt {1} {2}";

        /// <summary>
        /// Concatenates cpuinfo file to standard output.
        /// </summary>
        public const string CatProcCpuInfo = "cat /proc/cpuinfo";

        /// <summary>
        /// Concatenates CPU scaling governor to standard output.
        /// </summary>
        public const string CatScalingGovernor = "cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_governor";

        /// <summary>
        /// Concatenates config.txt file to standard output.
        /// </summary>
        public const string CatBootConfig = "cat /boot/config.txt";

        /// <summary>
        /// Displays free disk space.
        /// </summary>
        public const string Df = "df -T";

        /// <summary>
        /// Displays memory usage.
        /// </summary>
        public const string Free = "free --mega";

        /// <summary>
        /// Displays the Gpu memory.
        /// </summary>
        public const string GetMemGpu = "vcgencmd get_mem gpu";

        /// <summary>
        /// Lists processes running on the system.
        /// </summary>
        public const string Top = "top -bc -n 1";

        /// <summary>
        /// Gets the system hostname.
        /// </summary>
        public const string Hostnamectl = "hostnamectl";

        /// <summary>
        /// Prints group names a user is in.
        /// </summary>
        public const string Groups = "groups {0}";

        /// <summary>
        /// Displays how long the system has been up.
        /// </summary>
        public const string Uptime = "uptime -p";

        /// <summary>
        /// Stops a process from running.
        /// </summary>
        public const string SudoKill = "sudo kill {0}";

        /// <summary>
        /// Shows a process status.
        /// </summary>
        public const string PsUser = "ps -o user= -p {0}";

        /// <summary>
        /// Runs the stream editor over config.txt file.
        /// </summary>
        public const string SudoSedBootConfig = "sudo sed -i 's/{0}/{1}/' /boot/config.txt";

        /// <summary>
        /// Displays information on network interfaces.
        /// </summary>
        public const string Ifconfig = "ifconfig";

        /// <summary>
        /// Concatenates dev file to standard output.
        /// </summary>
        public const string CatProcNetDev = "cat /proc/net/dev";
    }
}
