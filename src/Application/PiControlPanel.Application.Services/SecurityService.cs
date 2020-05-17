namespace PiControlPanel.Application.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Authentication;

    public class SecurityService : ISecurityService
    {
        private readonly IUserAccountService userAccountService;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        
        public SecurityService(IUserAccountService userAccountService,
            IConfiguration configuration, ILogger logger)
        {
            this.userAccountService = userAccountService;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(UserAccount userAccount)
        {
            logger.Debug("Application layer -> SecurityService -> LoginAsync");

            if (userAccount == null ||
                string.IsNullOrWhiteSpace(userAccount.Username) ||
                string.IsNullOrWhiteSpace(userAccount.Password))
            {
                throw new BusinessException("Missing user account information");
            }

            var isUserAccountValid = await userAccountService.ValidateAsync(userAccount);
            if (!isUserAccountValid)
            {
                throw new BusinessException("Invalid user account");
            }

            return await this.GetLoginResponseAsync(userAccount);
        }

        public async Task<LoginResponse> GetLoginResponseAsync(UserAccount userAccount)
        {
            logger.Debug("Application layer -> SecurityService -> GetLoginResponseAsync");

            var jsonWebToken = await this.GenerateJwtSecurityTokenAsync(userAccount);
            var roleClaims = jsonWebToken.Claims.Where(c => c.Type == ClaimTypes.Role);

            return new LoginResponse()
            {
                Username = userAccount.Username,
                JsonWebToken = new JwtSecurityTokenHandler().WriteToken(jsonWebToken),
                Roles = roleClaims.Select(c => c.Value).ToList()
            };
        }

        private async Task<JwtSecurityToken> GenerateJwtSecurityTokenAsync(UserAccount userAccount)
        {
            logger.Debug("Application layer -> SecurityService -> GenerateJwtSecurityTokenAsync");
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = await this.CreateClaimsIdentityAsync(userAccount);

            return new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claimsIdentity.Claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentityAsync(UserAccount userAccount)
        {
            var identity = new ClaimsIdentity();

            if (userAccount != null)
            {
                logger.Info($"Creating claims for user {userAccount.Username}");
                identity.AddClaim(new Claim(CustomClaimTypes.Username, userAccount.Username));
                identity.AddClaim(new Claim(CustomClaimTypes.IsAnonymous, false.ToString()));
                identity.AddClaim(new Claim(CustomClaimTypes.IsAuthenticated, true.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, Roles.User));

                var isSuperUser = await this.userAccountService.IsSuperUserAsync(userAccount);
                if (isSuperUser)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, Roles.SuperUser));
                }
            }
            else
            {
                logger.Info("Creating claims for anonymous user");
                identity.AddClaim(new Claim(CustomClaimTypes.IsAnonymous, true.ToString()));
            }

            return identity;
        }
    }
}
