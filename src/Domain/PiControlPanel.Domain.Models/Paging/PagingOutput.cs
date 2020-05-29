namespace PiControlPanel.Domain.Models.Paging
{
    using System.Collections.Generic;

    /// <summary>
    /// The paging output model.
    /// </summary>
    /// <typeparam name="T">The model generic type parameter.</typeparam>
    public class PagingOutput<T>
    {
        /// <summary>
        /// Gets or sets the enumerable containing a set of objects.
        /// </summary>
        public IEnumerable<T> Result { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are more pages to be retrieved before the ones already retrieved.
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are more pages to be retrieved after the ones already retrieved.
        /// </summary>
        public bool HasNextPage { get; set; }
    }
}
