namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models;

    public interface IControlPanelService
    {
        Task<bool> ShutdownAsync(BusinessContext context);

        Task<bool> KillAsync(BusinessContext context, int processId);
    }
}
