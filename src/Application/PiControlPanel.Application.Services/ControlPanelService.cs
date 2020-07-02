namespace PiControlPanel.Application.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;
    using Infra = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;

    /// <inheritdoc/>
    public class ControlPanelService : IControlPanelService
    {
        private readonly Infra.IControlPanelService onDemandService;
        private readonly IOsService operatingSystemService;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelService"/> class.
        /// </summary>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="operatingSystemService">The application layer OsService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelService(
            Infra.IControlPanelService onDemandService,
            IOsService operatingSystemService,
            ILogger logger)
        {
            this.onDemandService = onDemandService;
            this.operatingSystemService = operatingSystemService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<bool> RebootAsync()
        {
            this.logger.Debug("Application layer -> ControlPanelService -> RebootAsync");
            return this.onDemandService.RebootAsync();
        }

        /// <inheritdoc/>
        public Task<bool> ShutdownAsync()
        {
            this.logger.Debug("Application layer -> ControlPanelService -> ShutdownAsync");
            return this.onDemandService.ShutdownAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync()
        {
            this.logger.Debug("Application layer -> ControlPanelService -> UpdateAsync");

            var updated = await this.onDemandService.UpdateAsync();
            if (updated)
            {
                await this.operatingSystemService.SaveAsync();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> KillAsync(UserContext context, int processId)
        {
            this.logger.Debug("Application layer -> ControlPanelService -> KillAsync");

            var isAuthorizedToKill = await this.IsAuthorizedToKillAsync(context, processId);
            if (!isAuthorizedToKill)
            {
                this.logger.Warn($"User '{context.Username}' is not authorized to kill process '{processId}'");
                return false;
            }

            return await this.onDemandService.KillAsync(context, processId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsAuthorizedToKillAsync(UserContext context, int processId)
        {
            if (context.IsSuperUser)
            {
                return true;
            }

            var processOwnerUsername = await this.onDemandService.GetProcessOwnerUsernameAsync(processId);
            return context.Username.Equals(processOwnerUsername);
        }

        /// <inheritdoc/>
        public Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel)
        {
            this.logger.Debug("Application layer -> ControlPanelService -> OverclockAsync");
            return this.onDemandService.OverclockAsync(cpuMaxFrequencyLevel);
        }

        /// <inheritdoc/>
        public Task<bool> StartSshAsync()
        {
            this.logger.Debug("Application layer -> ControlPanelService -> StartSshAsync");
            return this.onDemandService.StartSshAsync();
        }
    }
}
