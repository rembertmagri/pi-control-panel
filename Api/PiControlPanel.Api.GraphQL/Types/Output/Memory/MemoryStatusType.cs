namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryStatusType<T> : ObjectGraphType<T>
        where T : MemoryStatus
    {
        public MemoryStatusType()
        {
            Field(x => x.Used);
            Field(x => x.Free);
            //Field(x => x.DiskCache); TODO add if RAM
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
