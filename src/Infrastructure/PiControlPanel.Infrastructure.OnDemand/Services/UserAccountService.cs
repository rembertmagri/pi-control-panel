﻿namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Models.Authentication;
    using PiControlPanel.Infrastructure.OnDemand.Util;

    /// <inheritdoc/>
    public class UserAccountService : IUserAccountService
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountService"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public UserAccountService(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> ValidateAsync(UserAccount userAccount)
        {
            this.logger.Debug("Infra layer -> UserAccountService -> ValidateAsync");

            var catEtcShadowCommand = string.Format(
                BashCommands.SudoCatEtcShadow,
                userAccount.Username);
            var loginInfo = await catEtcShadowCommand.BashAsync();
            this.logger.Trace($"Result of '{catEtcShadowCommand}' command: '{loginInfo}'");

            if (string.IsNullOrWhiteSpace(loginInfo))
            {
                this.logger.Error($"User {userAccount.Username} not found");
                return false;
            }

            var parsedLoginInfo = loginInfo.Split(':');
            if (!userAccount.Username.Equals(parsedLoginInfo[0]))
            {
                this.logger.Error($"Found username {parsedLoginInfo[0]} different from searched {userAccount.Username}");
                return false;
            }

            var passwordInfo = parsedLoginInfo[1].Split('$');
            var openSslPasswdCommand = string.Format(
                BashCommands.OpenSslPasswd,
                passwordInfo[1],
                passwordInfo[2],
                userAccount.Password);
            var hashedPassword = await openSslPasswdCommand.BashAsync();
            this.logger.Trace($"Result of '{openSslPasswdCommand}' command: '{hashedPassword}'");
            if (!string.Equals(parsedLoginInfo[1], hashedPassword, StringComparison.InvariantCultureIgnoreCase))
            {
                this.logger.Error($"Hashed password {hashedPassword} different from existing hashed password {parsedLoginInfo[1]}");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> IsSuperUserAsync(UserAccount userAccount)
        {
            this.logger.Debug("Infra layer -> UserAccountService -> IsSuperUserAsync");

            var groupsCommand = string.Format(
                BashCommands.Groups,
                userAccount.Username);
            var result = await groupsCommand.BashAsync();
            this.logger.Trace($"Result of '{groupsCommand}' command: '{result}'");
            return result.Contains(" sudo ") || result.EndsWith(" sudo");
        }
    }
}
