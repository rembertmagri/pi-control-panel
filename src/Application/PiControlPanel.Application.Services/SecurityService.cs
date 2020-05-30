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

    /// <inheritdoc/>
    public class SecurityService : ISecurityService
    {
        private readonly IUserAccountService userAccountService;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        /// <param name="userAccountService">The infrastructure layer persistence service.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public SecurityService(
            IUserAccountService userAccountService,
            IConfiguration configuration,
            ILogger logger)
        {
            this.userAccountService = userAccountService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<LoginResponse> LoginAsync(UserAccount userAccount)
        {
            this.logger.Debug("Application layer -> SecurityService -> LoginAsync");

            if (userAccount == null ||
                string.IsNullOrWhiteSpace(userAccount.Username) ||
                string.IsNullOrWhiteSpace(userAccount.Password))
            {
                throw new BusinessException("Missing user account information");
            }

            var isUserAccountValid = await this.userAccountService.ValidateAsync(userAccount);
            if (!isUserAccountValid)
            {
                throw new BusinessException("Invalid user account");
            }

            return await this.GetLoginResponseAsync(userAccount);
        }

        /// <inheritdoc/>
        public async Task<LoginResponse> GetLoginResponseAsync(UserAccount userAccount)
        {
            this.logger.Debug("Application layer -> SecurityService -> GetLoginResponseAsync");

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
            this.logger.Debug("Application layer -> SecurityService -> GenerateJwtSecurityTokenAsync");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = await this.CreateClaimsIdentityAsync(userAccount);

            return new JwtSecurityToken(
                this.configuration["Jwt:Issuer"],
                this.configuration["Jwt:Audience"],
                claimsIdentity.Claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentityAsync(UserAccount userAccount)
        {
            var identity = new ClaimsIdentity();

            if (userAccount != null)
            {
                this.logger.Info($"Creating claims for user {userAccount.Username}");
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
                this.logger.Info("Creating claims for anonymous user");
                identity.AddClaim(new Claim(CustomClaimTypes.IsAnonymous, true.ToString()));
            }

            return identity;
        }
    }
}
