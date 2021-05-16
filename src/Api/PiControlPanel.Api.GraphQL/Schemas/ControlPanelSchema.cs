namespace PiControlPanel.Api.GraphQL.Schemas
{
    using System;
    using System.Collections.Generic;
    using global::GraphQL.Instrumentation;
    using global::GraphQL.Types;
    using global::GraphQL.Utilities;

    /// <summary>
    /// The root GraphQL schema.
    /// </summary>
    public class ControlPanelSchema : Schema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelSchema"/> class.
        /// </summary>
        /// <param name="provider">Resolver for the Queries and Mutations.</param>
        /// <param name="middlewares">A list of all registered field middlewares.</param>
        /// <param name="nodeVisitors">A list of all registered node visitors.</param>
        public ControlPanelSchema(
            IServiceProvider provider,
            IEnumerable<IFieldMiddleware> middlewares,
            IEnumerable<ISchemaNodeVisitor> nodeVisitors)
            : base(provider)
        {
            this.Query = provider.GetService(typeof(ControlPanelQuery)) as IObjectGraphType;
            this.Mutation = provider.GetService(typeof(ControlPanelMutation)) as IObjectGraphType;
            this.Subscription = provider.GetService(typeof(ControlPanelSubscription)) as IObjectGraphType;

            foreach (var middleware in middlewares)
            {
                this.FieldMiddleware.Use(middleware);
            }

            foreach (var nodeVisitor in nodeVisitors)
            {
                this.RegisterVisitor(nodeVisitor);
            }
        }
    }
}
