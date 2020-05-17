namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;

    public interface IControlPanelService
    {
        Task<bool> RebootAsync();

        Task<bool> ShutdownAsync();

        Task<bool> UpdateAsync();

        Task<bool> KillAsync(BusinessContext context, int processId);

        Task<string> GetProcessOwnerUsernameAsync(int processId);

        Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel);
    }
}
