namespace PiControlPanel.Application.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models;
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
        
        public Task<bool> ShutdownAsync(BusinessContext context)
        {
            logger.Info("Application layer -> ControlPanelService -> ShutdownAsync");
            return onDemandService.ShutdownAsync(context);
        }

        public Task<bool> KillAsync(BusinessContext context, int processId)
        {
            logger.Info("Application layer -> ControlPanelService -> KillAsync");
            return onDemandService.KillAsync(context, processId);
        }
    }
}
