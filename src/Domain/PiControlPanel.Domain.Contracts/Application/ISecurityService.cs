namespace PiControlPanel.Domain.Contracts.Application
{
    using PiControlPanel.Domain.Models.Authentication;
    using System.Threading.Tasks;

    public interface ISecurityService
    {
        Task<LoginResponse> LoginAsync(UserAccount userAccount);

        Task<LoginResponse> GetLoginResponseAsync(UserAccount userAccount);
    }
}
