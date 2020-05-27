namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL;
    using global::GraphQL.Types;

    public class ControlPanelSchema : Schema
    {
        public ControlPanelSchema(IDependencyResolver dependencyResolver)
            : base(dependencyResolver)
        {
            this.Query = dependencyResolver.Resolve<ControlPanelQuery>();
            this.Mutation = dependencyResolver.Resolve<ControlPanelMutation>();
            this.Subscription = dependencyResolver.Resolve<ControlPanelSubscription>();
        }
    }
}
