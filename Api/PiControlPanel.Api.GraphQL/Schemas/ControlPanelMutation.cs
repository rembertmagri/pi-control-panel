namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL.Authorization;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;

    public class ControlPanelMutation : ObjectGraphType
    {
        public ControlPanelMutation(IControlPanelService controlPanelService, ILogger logger)
        {
            this.AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            FieldAsync<BooleanGraphType>(
                "Shutdown",
                resolve: async context =>
                {
                    logger.Info("Shutdown mutation");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await controlPanelService.ShutdownAsync(businessContext);
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            FieldAsync<BooleanGraphType>(
                "Kill",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "ProcessId" }
                ),
                resolve: async context =>
                {
                    logger.Info("Kill mutation");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var processId = context.GetArgument<int>("processId");

                    return await controlPanelService.KillAsync(businessContext, processId);
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);
        }
    }
}
