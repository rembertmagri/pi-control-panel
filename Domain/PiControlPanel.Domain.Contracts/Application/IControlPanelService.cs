namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models;

    public interface IControlPanelService
    {
        Task<bool> ShutdownAsync();

        Task<bool> KillAsync(BusinessContext context, int processId);

        Task<bool> IsAuthorizedToKillAsync(BusinessContext context, int processId);
    }
}
