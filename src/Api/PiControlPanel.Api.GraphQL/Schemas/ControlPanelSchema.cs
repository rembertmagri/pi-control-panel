namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL;
    using global::GraphQL.Types;

    /// <summary>
    /// The root GraphQL schema.
    /// </summary>
    public class ControlPanelSchema : Schema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelSchema"/> class.
        /// </summary>
        /// <param name="dependencyResolver">GraphQL dependency resolver.</param>
        public ControlPanelSchema(IDependencyResolver dependencyResolver)
            : base(dependencyResolver)
        {
            this.Query = dependencyResolver.Resolve<ControlPanelQuery>();
            this.Mutation = dependencyResolver.Resolve<ControlPanelMutation>();
            this.Subscription = dependencyResolver.Resolve<ControlPanelSubscription>();
        }
    }
}
