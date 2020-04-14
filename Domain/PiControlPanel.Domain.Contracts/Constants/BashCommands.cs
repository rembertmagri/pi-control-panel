namespace PiControlPanel.Domain.Contracts.Constants
{
    public class BashCommands
    {
        public const string MeasureTemp = "vcgencmd measure_temp";
        public const string CatCpuFreqStats = "cat /sys/devices/system/cpu/cpu0/cpufreq/stats/time_in_state";
        public const string SudoReboot = "sudo reboot";
        public const string SudoShutdown = "sudo shutdown -h now";
        public const string SudoAptgetUpdade = "sudo apt-get update";
        public const string SudoAptgetUpgrade = "sudo apt-get upgrade -y";
        public const string SudoAptgetAutoremove = "sudo apt-get autoremove -y";
        public const string SudoAptgetAutoclean = "sudo apt-get autoclean";
        public const string SudoCatEtcShadow = "sudo cat /etc/shadow | grep {0}";
        public const string OpenSslPasswd = "openssl passwd -{0} -salt {1} {2}";
        public const string CatProcCpuInfo = "cat /proc/cpuinfo";
        public const string CatBootConfig = "cat /boot/config.txt";
        public const string Df = "df -T";
        public const string Free = "free --mega";
        public const string GetMemGpu = "vcgencmd get_mem gpu";
        public const string Top = "top -bc -n 1";
        public const string Hostnamectl = "hostnamectl";
        public const string Groups = "groups {0}";
        public const string Uptime = "uptime -p";
        public const string SudoKill = "sudo kill {0}";
        public const string PsUser = "ps -o user= -p {0}";
        public const string SudoSedBootConfig = "sudo sed -i 's/{0}/{1}/' /boot/config.txt";
    }
}
