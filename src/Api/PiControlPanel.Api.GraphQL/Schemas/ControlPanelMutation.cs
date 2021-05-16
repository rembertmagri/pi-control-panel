namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL;
    using global::GraphQL.MicrosoftDI;
    using global::GraphQL.Types;
    using PiControlPanel.Api.GraphQL.Types.Input;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;

    /// <summary>
    /// The root mutation GraphQL type.
    /// </summary>
    public class ControlPanelMutation : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelMutation"/> class.
        /// </summary>
        public ControlPanelMutation()
        {
            this.AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("Reboot")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    return await controlPanelService.RebootAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("Shutdown")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    return await controlPanelService.ShutdownAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("Update")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    return await controlPanelService.UpdateAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("Kill")
                .Argument<NonNullGraphType<IntGraphType>, int>("ProcessId", "the process identitfier")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    var userContext = context.UserContext["UserContext"] as UserContext;
                    var processId = context.GetArgument<int>("processId");

                    return await controlPanelService.KillAsync(userContext, processId);
                })
                .AuthorizeWith(AuthorizationPolicyName.AuthenticatedPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("Overclock")
                .Argument<NonNullGraphType<CpuMaxFrequencyLevelType>, CpuMaxFrequencyLevel>("CpuMaxFrequencyLevel", "the CPU max level")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    var cpuMaxFrequencyLevel = context.GetArgument<CpuMaxFrequencyLevel>("cpuMaxFrequencyLevel");

                    return await controlPanelService.OverclockAsync(cpuMaxFrequencyLevel);
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);

            this.Field<BooleanGraphType, bool>()
                .Name("StartSsh")
                .Resolve()
                .WithScope()
                .WithService<IControlPanelService>()
                .ResolveAsync(async (context, controlPanelService) =>
                {
                    return await controlPanelService.StartSshAsync();
                })
                .AuthorizeWith(AuthorizationPolicyName.SuperUserPolicy);
        }
    }
}
