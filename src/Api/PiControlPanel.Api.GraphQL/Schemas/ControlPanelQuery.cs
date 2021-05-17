namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL.Authorization;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Api.GraphQL.Types.Output;
    using PiControlPanel.Api.GraphQL.Types.Input;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models.Authentication;
    using PiControlPanel.Api.GraphQL.Types.Output.Authentication;

    /// <summary>
    /// The root query GraphQL type.
    /// </summary>
    public class ControlPanelQuery : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelQuery"/> class.
        /// </summary>
        /// <param name="securityService">The application layer SecurityService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelQuery(ISecurityService securityService, ILogger logger)
        {
            this.FieldAsync<LoginResponseType>(
                "Login",
                arguments: new QueryArguments(
                    new QueryArgument<UserAccountInputType> { Name = "UserAccount" }),
                resolve: async context =>
                {
                    logger.Info("Login query");

                    var userAccount = context.GetArgument<UserAccount>("userAccount");

                    return await securityService.LoginAsync(userAccount);
                });

            this.FieldAsync<LoginResponseType>(
                "RefreshToken",
                resolve: async context =>
                {
                    logger.Info("RefreshToken query");

                    var graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var userContext = graphQLUserContext.GetUserContext();

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
                    logger.Info("RaspberryPi query");

                    // Retuning empty object to make GraphQL resolve the RaspberryPiType fields
                    // https://graphql-dotnet.github.io/docs/getting-started/query-organization/
                    return new { };
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);
        }
    }
}
