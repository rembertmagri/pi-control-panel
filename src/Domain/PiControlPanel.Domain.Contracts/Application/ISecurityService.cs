namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Authentication;

    public interface ISecurityService
    {
        Task<LoginResponse> LoginAsync(UserAccount userAccount);

        Task<LoginResponse> GetLoginResponseAsync(UserAccount userAccount);
    }
}
