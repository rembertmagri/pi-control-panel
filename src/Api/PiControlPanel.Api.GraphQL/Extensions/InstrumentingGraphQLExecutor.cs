namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::GraphQL;
    using global::GraphQL.Execution;
    using global::GraphQL.Instrumentation;
    using global::GraphQL.Server;
    using global::GraphQL.Server.Internal;
    using global::GraphQL.Types;
    using global::GraphQL.Validation;
    using Microsoft.Extensions.Options;

    /// <inheritdoc/>
    public class InstrumentingGraphQLExecutor<TSchema> : DefaultGraphQLExecuter<TSchema>
        where TSchema : ISchema
    {
        private readonly GraphQLOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentingGraphQLExecutor{TSchema}"/> class.
        /// </summary>
        /// <param name="schema">The GraphQL schema.</param>
        /// <param name="documentExecuter">The GraphQL document executer.</param>
        /// <param name="options">The GraphQL options.</param>
        /// <param name="listeners">The GraphQL document execution listeners.</param>
        /// <param name="validationRules">The GraphQL validation rules.</param>
        public InstrumentingGraphQLExecutor(
            TSchema schema,
            IDocumentExecuter documentExecuter,
            IOptions<GraphQLOptions> options,
            IEnumerable<IDocumentExecutionListener> listeners,
            IEnumerable<IValidationRule> validationRules)
            : base(schema, documentExecuter, options, listeners, validationRules) =>
            this.options = options.Value;

        /// <inheritdoc/>
        public override async Task<ExecutionResult> ExecuteAsync(
            string operationName,
            string query,
            Inputs variables,
            object context,
            CancellationToken cancellationToken = default)
        {
            var result = await base.ExecuteAsync(operationName, query, variables, context, cancellationToken);

            if (this.options.EnableMetrics)
            {
                // Add instrumentation data showing how long field resolvers take to execute to the JSON response in
                // Apollo Tracing format. Apollo Engine can use the Apollo Tracing data to produce nice charts showing this
                // information. See https://www.apollographql.com/engine/
                //                result.EnrichWithApolloTracing(DateTime.UtcNow);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override ExecutionOptions GetOptions(
            string operationName,
            string query,
            Inputs variables,
            object context,
            CancellationToken cancellationToken)
        {
            var options = base.GetOptions(operationName, query, variables, context, cancellationToken);

            if (this.options.EnableMetrics)
            {
                // Add instrumentation to measure how long field resolvers take to execute.
                options.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
            }

            return options;
        }
    }
}
