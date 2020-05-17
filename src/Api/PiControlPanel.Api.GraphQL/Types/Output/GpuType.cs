namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    public class GpuType : ObjectGraphType<Gpu>
    {
        public GpuType()
        {
            Field(x => x.Memory);
            Field(x => x.Frequency);
        }
    }
}
