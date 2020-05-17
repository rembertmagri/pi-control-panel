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

    public class ControlPanelQuery : ObjectGraphType
    {
        public ControlPanelQuery(ISecurityService securityService, ILogger logger)
        {
            FieldAsync<LoginResponseType>(
                "Login",
                arguments: new QueryArguments(
                    new QueryArgument<UserAccountInputType> { Name = "UserAccount" }
                ),
                resolve: async context =>
                {
                    logger.Info("Login query");
                    var userAccount = context.GetArgument<UserAccount>("userAccount");

                    return await securityService.LoginAsync(userAccount);
                });

            FieldAsync<LoginResponseType>(
                "RefreshToken",
                resolve: async context =>
                {
                    logger.Info("RefreshToken query");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var userAccount = new UserAccount()
                    {
                        Username = businessContext.Username
                    };

                    return await securityService.GetLoginResponseAsync(userAccount);
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            Field<RaspberryPiType>(
                "RaspberryPi",
                resolve: context =>
                {
                    logger.Info("RaspberryPi query");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    // Retuning empty object to make GraphQL resolve the RaspberryPiType fields
                    // https://graphql-dotnet.github.io/docs/getting-started/query-organization/
                    return new { };
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);
        }
    }
}
