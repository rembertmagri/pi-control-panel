namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL;
    using global::GraphQL.Types;
    using PiControlPanel.Api.GraphQL.Types.Output;
    using PiControlPanel.Api.GraphQL.Types.Input;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models.Authentication;
    using PiControlPanel.Api.GraphQL.Types.Output.Authentication;
    using PiControlPanel.Domain.Models;

    /// <summary>
    /// The root query GraphQL type.
    /// </summary>
    public class ControlPanelQuery : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelQuery"/> class.
        /// </summary>
        /// <param name="securityService">The application layer SecurityService.</param>
        public ControlPanelQuery(ISecurityService securityService)
        {
            this.FieldAsync<LoginResponseType>(
                "Login",
                arguments: new QueryArguments(
                    new QueryArgument<UserAccountInputType> { Name = "UserAccount" }),
                resolve: async context =>
                {
                    var userAccount = context.GetArgument<UserAccount>("userAccount");

                    return await securityService.LoginAsync(userAccount);
                });

            this.FieldAsync<LoginResponseType>(
                "RefreshToken",
                resolve: async context =>
                {
                    var userContext = context.UserContext["UserContext"] as UserContext;

                    var userAccount = new UserAccount()
                    {
                        Username = userContext.Username,
                        Password = userContext.Password
                    };

                    return await securityService.GetLoginResponseAsync(userAccount);
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            this.Field<RaspberryPiType>(
                "RaspberryPi",
                resolve: context =>
                {
                    // Retuning empty object to make GraphQL resolve the RaspberryPiType fields
                    // https://graphql-dotnet.github.io/docs/getting-started/query-organization/
                    return new { };
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);
        }
    }
}
