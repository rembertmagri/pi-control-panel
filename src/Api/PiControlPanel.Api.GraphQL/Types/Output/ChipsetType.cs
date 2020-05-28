namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// The Chipset GraphQL output type.
    /// </summary>
    public class ChipsetType : ObjectGraphType<Chipset>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipsetType"/> class.
        /// </summary>
        public ChipsetType()
        {
            this.Field(x => x.Version);
            this.Field(x => x.Revision);
            this.Field(x => x.Serial);
            this.Field(x => x.Model);
        }
    }
}
