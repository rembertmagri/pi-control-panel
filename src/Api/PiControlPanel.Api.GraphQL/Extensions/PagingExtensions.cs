namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Linq;
    using global::GraphQL.Builders;
    using global::GraphQL.Types.Relay.DataObjects;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Domain.Models.Paging;

    public static class PagingExtensions
    {
        public static PagingInput GetPagingInput<T>(this ResolveConnectionContext<T> context)
        {
            return new PagingInput()
            {
                First = context.First,
                After = context.After,
                Last = context.Last,
                Before = context.Before
            };
        }

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
