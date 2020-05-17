namespace PiControlPanel.Domain.Models.Paging
{
    using System.Collections.Generic;

    public class PagingOutput<T>
    {
        public IEnumerable<T> Result { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }
    }
}
