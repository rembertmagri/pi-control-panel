namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    public class ChipsetType : ObjectGraphType<Chipset>
    {
        public ChipsetType()
        {
            Field(x => x.Version);
            Field(x => x.Revision);
            Field(x => x.Serial);
            Field(x => x.Model);
        }
    }
}
