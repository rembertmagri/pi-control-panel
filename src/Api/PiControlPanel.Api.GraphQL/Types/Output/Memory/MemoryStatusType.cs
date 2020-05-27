namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryStatusType<T> : ObjectGraphType<T>
        where T : MemoryStatus
    {
        public MemoryStatusType()
        {
            this.Field<DateTimeGraphType>("dateTime");
            this.Field(x => x.Used);
            this.Field(x => x.Free);
            this.Field<IntGraphType>(
                "DiskCache",
                resolve: context => {
                    if (typeof(T) == typeof(RandomAccessMemoryStatus))
                    {
                        return (context.Source as RandomAccessMemoryStatus).DiskCache;
                    }

                    return 0;
            });
        }
    }
}
