namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Contracts.Util;
    using PiControlPanel.Domain.Models;
    
    public class ControlPanelService : IControlPanelService
    {
        private readonly ILogger logger;

        public ControlPanelService(ILogger logger)
        {
            this.logger = logger;
        }

        public Task<bool> RebootAsync()
        {
            logger.Info("Infra layer -> ControlPanelService -> RebootAsync");
            var result = BashCommands.SudoReboot.Bash();
            logger.Debug($"Result of '{BashCommands.SudoReboot}' command: '{result}'");
            return Task.FromResult(true);
        }

        public Task<bool> ShutdownAsync()
        {
            logger.Info("Infra layer -> ControlPanelService -> ShutdownAsync");
            var result = BashCommands.SudoShutdown.Bash();
            logger.Debug($"Result of '{BashCommands.SudoShutdown}' command: '{result}'");
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync()
        {
            logger.Info("Infra layer -> ControlPanelService -> UpdateAsync");
            var result = BashCommands.SudoAptgetUpdade.Bash();
            logger.Debug($"Result of '{BashCommands.SudoAptgetUpdade}' command: '{result}'");
            result = BashCommands.SudoAptgetUpgrade.Bash();
            logger.Debug($"Result of '{BashCommands.SudoAptgetUpgrade}' command: '{result}'");

            string lastLine = result
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault();
            logger.Info($"Firmware update summary: '{lastLine}'");
            if ("0 upgraded, 0 newly installed, 0 to remove and 0 not upgraded."
                .Equals(lastLine))
            {
                logger.Info("Firmware already up-to-date, no need to update.");
                return Task.FromResult(false);
            }
            
            result = BashCommands.SudoAptgetAutoremove.Bash();
            logger.Debug($"Result of '{BashCommands.SudoAptgetAutoremove}' command: '{result}'");
            result = BashCommands.SudoAptgetAutoclean.Bash();
            logger.Debug($"Result of '{BashCommands.SudoAptgetAutoclean}' command: '{result}'");

            return this.RebootAsync();
        }

        public Task<bool> KillAsync(BusinessContext context, int processId)
        {
            logger.Info("Infra layer -> ControlPanelService -> KillAsync");

            var sudoKillCommand = string.Format(BashCommands.SudoKill, processId);
            var result = sudoKillCommand.Bash();

            if (!string.IsNullOrEmpty(result))
            {
                logger.Warn($"Result of '{sudoKillCommand}' command: '{result}'");
                return Task.FromResult(false);
            }

            logger.Debug($"Result of '{sudoKillCommand}' command is empty, success");
            return Task.FromResult(true);
        }

        public Task<string> GetProcessOwnerUsernameAsync(int processId)
        {
            logger.Info("Infra layer -> ControlPanelService -> GetProcessOwnerUsernameAsync");

            var psUserCommand = string.Format(BashCommands.PsUser, processId);
            var result = psUserCommand.Bash();

            if (string.IsNullOrEmpty(result))
            {
                logger.Warn($"Result of '{psUserCommand}' command is empty, process '{processId}' doesn't exist");
                return Task.FromResult(string.Empty);
            }

            logger.Debug($"Result of '{psUserCommand}' command: '{result}'");
            return Task.FromResult(result);
        }
    }
}
