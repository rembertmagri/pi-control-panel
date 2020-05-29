namespace PiControlPanel.Domain.Models.Paging
{
    /// <summary>
    /// The paging input model.
    /// </summary>
    public class PagingInput
    {
        /// <summary>
        /// Gets or sets the number of first occurences to be retrieved.
        /// </summary>
        public int? First { get; set; }

        /// <summary>
        /// Gets or sets the first cursor after the last retrieved value.
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Gets or sets the number of last occurences to be retrieved.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Gets or sets the last cursor before the first retrieved value.
        /// </summary>
        public string Before { get; set; }
    }
}
