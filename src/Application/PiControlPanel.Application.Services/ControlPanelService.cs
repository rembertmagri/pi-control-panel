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
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelService"/> class.
        /// </summary>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelService(Infra.IControlPanelService onDemandService, ILogger logger)
        {
            this.onDemandService = onDemandService;
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
        public Task<bool> UpdateAsync()
        {
            this.logger.Debug("Application layer -> ControlPanelService -> UpdateAsync");
            return this.onDemandService.UpdateAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> KillAsync(BusinessContext context, int processId)
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
        public async Task<bool> IsAuthorizedToKillAsync(BusinessContext context, int processId)
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
    }
}
