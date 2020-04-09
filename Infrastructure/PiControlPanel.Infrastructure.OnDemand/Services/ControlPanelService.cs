namespace PiControlPanel.Infrastructure.OnDemand.Services
{
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

        public Task<bool> ShutdownAsync()
        {
            logger.Info("Infra layer -> ControlPanelService -> ShutdownAsync");
            var result = BashCommands.SudoShutdown.Bash();
            logger.Debug($"Result of '{BashCommands.SudoShutdown}' command: '{result}'");
            return Task.FromResult(true);
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
                logger.Warn($"Result of '{psUserCommand}' command is empty, success");
                return Task.FromResult(string.Empty);
            }

            logger.Debug($"Result of '{psUserCommand}' command: '{result}'");
            return Task.FromResult(result);
        }
    }
}
