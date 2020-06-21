namespace PiControlPanel.Domain.Contracts.Util
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Utility class that extends string to execute bash commands.
    /// </summary>
    public static class ShellExtensions
    {
        /// <summary>
        /// Executes a bash command and returns the result.
        /// </summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <returns>The result of the bash command.</returns>
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result == null ? string.Empty : result.TrimEnd(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Executes a bash command in background.
        /// </summary>
        /// <param name="cmd">The command to be executed.</param>
        public static void BashBg(this string cmd)
        {
            Task.Run(() => Bash(cmd));
        }
    }
}
