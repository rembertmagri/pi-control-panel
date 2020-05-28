namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// The Gpu GraphQL output type.
    /// </summary>
    public class GpuType : ObjectGraphType<Gpu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpuType"/> class.
        /// </summary>
        public GpuType()
        {
            this.Field(x => x.Memory);
            this.Field(x => x.Frequency);
        }
    }
}
