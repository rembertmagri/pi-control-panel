namespace PiControlPanel.Infrastructure.OnDemand.Util
{
    using System;
    using System.Threading.Tasks;
    using CliWrap;
    using CliWrap.Buffered;
    using CliWrap.Exceptions;
    using PiControlPanel.Domain.Models;

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
        public static async Task<string> BashAsync(this string cmd)
        {
            var command = Cli.Wrap("/bin/bash").WithArguments(new[] { "-c", cmd });

            try
            {
                var result = await command.ExecuteBufferedAsync();
                return result.StandardOutput.TrimEnd(Environment.NewLine.ToCharArray());
            }
            catch (CommandExecutionException ex)
            {
                throw new BusinessException($"Error running '{cmd}' command", ex);
            }
        }
    }
}