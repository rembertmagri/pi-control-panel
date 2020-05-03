﻿namespace PiControlPanel.Application.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;
    using Infra = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;

    public class ControlPanelService : IControlPanelService
    {
        private readonly Infra.IControlPanelService onDemandService;
        private readonly ILogger logger;

        public ControlPanelService(Infra.IControlPanelService onDemandService, ILogger logger)
        {
            this.onDemandService = onDemandService;
            this.logger = logger;
        }

        public Task<bool> RebootAsync()
        {
            logger.Trace("Application layer -> ControlPanelService -> RebootAsync");
            return onDemandService.RebootAsync();
        }

        public Task<bool> ShutdownAsync()
        {
            logger.Trace("Application layer -> ControlPanelService -> ShutdownAsync");
            return onDemandService.ShutdownAsync();
        }

        public Task<bool> UpdateAsync()
        {
            logger.Trace("Application layer -> ControlPanelService -> UpdateAsync");
            return onDemandService.UpdateAsync();
        }

        public async Task<bool> KillAsync(BusinessContext context, int processId)
        {
            logger.Trace("Application layer -> ControlPanelService -> KillAsync");

            var isAuthorizedToKill = await this.IsAuthorizedToKillAsync(context, processId);
            if (!isAuthorizedToKill)
            {
                logger.Warn($"User '{context.Username}' is not authorized to kill process '{processId}'");
                return false;
            }

            return await onDemandService.KillAsync(context, processId);
        }

        public async Task<bool> IsAuthorizedToKillAsync(BusinessContext context, int processId)
        {
            if (context.IsSuperUser)
            {
                return true;
            }

            var processOwnerUsername = await onDemandService.GetProcessOwnerUsernameAsync(processId);
            return context.Username.Equals(processOwnerUsername);
        }

        public Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel)
        {
            logger.Trace("Application layer -> ControlPanelService -> OverclockAsync");
            return onDemandService.OverclockAsync(cpuMaxFrequencyLevel);
        }
    }
}
