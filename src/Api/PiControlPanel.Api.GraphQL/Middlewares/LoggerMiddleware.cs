namespace PiControlPanel.Api.GraphQL.Middlewares
{
    using System.Threading.Tasks;
    using global::GraphQL.Instrumentation;
    using global::GraphQL.Language.AST;
    using global::GraphQL;
    using NLog;
    using PiControlPanel.Domain.Models;

    /// <summary>
    /// Middleware to log GraphQL requests.
    /// </summary>
    public class LoggerMiddleware : IFieldMiddleware
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logger service reference.</param>
        public LoggerMiddleware(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<object> Resolve(IResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            var userContext = context.UserContext["UserContext"] as UserContext;
            var fieldDefinition = context.FieldDefinition;

            switch (context.Operation.OperationType)
            {
                case OperationType.Query:
                    this.logger.Info($"Query {fieldDefinition.Name} for user {userContext.Username}");
                    break;
                case OperationType.Mutation:
                    this.logger.Info($"Mutation {fieldDefinition.Name} for user {userContext.Username}");
                    break;
                case OperationType.Subscription:
                    this.logger.Info($"Subscription {fieldDefinition.Name} for user {userContext.Username}");
                    break;
                default:
                    break;
            }

            return await next(context);
        }
    }
}
