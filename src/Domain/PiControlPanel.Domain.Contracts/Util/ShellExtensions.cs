namespace PiControlPanel.Domain.Contracts.Util
{
    using System;
    using System.Diagnostics;

    public static class ShellExtensions
    {
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
    }
}
