namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Linq;
    using global::GraphQL.Builders;
    using global::GraphQL.Types.Relay.DataObjects;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Contains extension methods to handle paging with GraphQL connections.
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        /// Creates paging input object from GraphQL connection context.
        /// </summary>
        /// <typeparam name="T">The model generic type parameter.</typeparam>
        /// <param name="context">The GraphQL connection context.</param>
        /// <returns>The paging input object.</returns>
        public static PagingInput GetPagingInput<T>(this IResolveConnectionContext<T> context)
        {
            return new PagingInput()
            {
                First = context.First,
                After = context.After,
                Last = context.Last,
                Before = context.Before
            };
        }

        /// <summary>
        /// Creates a GraphQL connection from a paging output object.
        /// </summary>
        /// <typeparam name="T">The model generic type parameter.</typeparam>
        /// <param name="pagingOutput">The paging output object.</param>
        /// <returns>The GraphQL connection.</returns>
        public static Connection<T> ToConnection<T>(this PagingOutput<T> pagingOutput)
            where T : BaseTimedObject
        {
            var edges = pagingOutput.Result.Select((status, i) => new Edge<T>
            {
                Node = status,
                Cursor = status.ID.ToString()
            }).ToList();

            return new Connection<T>()
            {
                Edges = edges,
                TotalCount = pagingOutput.TotalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = edges.FirstOrDefault()?.Cursor,
                    EndCursor = edges.LastOrDefault()?.Cursor,
                    HasPreviousPage = pagingOutput.HasPreviousPage,
                    HasNextPage = pagingOutput.HasNextPage,
                }
            };
        }
    }
}
