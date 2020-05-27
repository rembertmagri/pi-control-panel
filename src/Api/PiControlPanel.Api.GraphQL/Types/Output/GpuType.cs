namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    public class GpuType : ObjectGraphType<Gpu>
    {
        public GpuType()
        {
            this.Field(x => x.Memory);
            this.Field(x => x.Frequency);
        }
    }
}
