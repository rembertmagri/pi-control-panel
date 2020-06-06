namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL.Authorization;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Api.GraphQL.Types.Input;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models.Enums;

    /// <summary>
    /// The root mutation GraphQL type.
    /// </summary>
    public class ControlPanelMutation : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelMutation"/> class.
        /// </summary>
        /// <param name="controlPanelService">The application layer ControlPanelService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelMutation(IControlPanelService controlPanelService, ILogger logger)
        {
            this.AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            this.FieldAsync<BooleanGraphType>(
                "Reboot",
                resolve: async context =>
                {
                    logger.Info("Reboot mutation");

                    return await controlPanelService.RebootAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.FieldAsync<BooleanGraphType>(
                "Shutdown",
                resolve: async context =>
                {
                    logger.Info("Shutdown mutation");

                    return await controlPanelService.ShutdownAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.FieldAsync<BooleanGraphType>(
                "Kill",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "ProcessId" }),
                resolve: async context =>
                {
                    logger.Info("Kill mutation");

                    var graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var userContext = graphQLUserContext.GetUserContext();

                    var processId = context.GetArgument<int>("processId");

                    return await controlPanelService.KillAsync(userContext, processId);
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            this.FieldAsync<BooleanGraphType>(
                "Overclock",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CpuMaxFrequencyLevelType>> { Name = "CpuMaxFrequencyLevel" }),
                resolve: async context =>
                {
                    logger.Info("Overclock mutation");

                    var cpuMaxFrequencyLevel = context.GetArgument<CpuMaxFrequencyLevel>("cpuMaxFrequencyLevel");

                    return await controlPanelService.OverclockAsync(cpuMaxFrequencyLevel);
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);
        }
    }
}
