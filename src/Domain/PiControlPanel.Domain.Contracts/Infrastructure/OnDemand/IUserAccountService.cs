namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Authentication;

    public interface IUserAccountService
    {
        Task<bool> ValidateAsync(UserAccount userAccount);

        Task<bool> IsSuperUserAsync(UserAccount userAccount);
    }
}
