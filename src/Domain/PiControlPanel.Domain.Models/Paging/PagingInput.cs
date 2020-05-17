namespace PiControlPanel.Domain.Models.Paging
{
    public class PagingInput
    {
        public int? First { get; set; }

        public string After { get; set; }

        public int? Last { get; set; }

        public string Before { get; set; }
    }
}
