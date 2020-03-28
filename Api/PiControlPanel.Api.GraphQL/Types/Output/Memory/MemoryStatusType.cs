namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryStatusType<T> : ObjectGraphType<T>
        where T : MemoryStatus
    {
        public MemoryStatusType()
        {
            Field<DateTimeGraphType>("dateTime");
            Field(x => x.Used);
            Field(x => x.Free);
            Field<IntGraphType>(
                "DiskCache",
                resolve: context => {
                    if(typeof(T) == typeof(RandomAccessMemoryStatus))
                    {
                        return (context.Source as RandomAccessMemoryStatus).DiskCache;
                    }
                    return 0;
            });
        }
    }
}
