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
                "Reboot",
                resolve: async context =>
                {
                    logger.Info("Reboot mutation");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await controlPanelService.RebootAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            FieldAsync<BooleanGraphType>(
                "Shutdown",
                resolve: async context =>
                {
                    logger.Info("Shutdown mutation");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await controlPanelService.ShutdownAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            FieldAsync<BooleanGraphType>(
                "Update",
                resolve: async context =>
                {
                    logger.Info("Update mutation");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await controlPanelService.UpdateAsync();
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
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);
        }
    }
}
