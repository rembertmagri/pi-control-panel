namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    public class ChipsetType : ObjectGraphType<Chipset>
    {
        public ChipsetType()
        {
            this.Field(x => x.Version);
            this.Field(x => x.Revision);
            this.Field(x => x.Serial);
            this.Field(x => x.Model);
        }
    }
}
