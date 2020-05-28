namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// The MemoryStatusType GraphQL output type.
    /// </summary>
    /// <typeparam name="T">The MemoryStatus generic type parameter.</typeparam>
    public class MemoryStatusType<T> : ObjectGraphType<T>
        where T : MemoryStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStatusType{T}"/> class.
        /// </summary>
        public MemoryStatusType()
        {
            this.Field<DateTimeGraphType>("dateTime");
            this.Field(x => x.Used);
            this.Field(x => x.Free);
            this.Field<IntGraphType>(
                "DiskCache",
                resolve: context =>
                {
                    if (typeof(T) == typeof(RandomAccessMemoryStatus))
                    {
                        return (context.Source as RandomAccessMemoryStatus).DiskCache;
                    }

                    return 0;
                });
        }
    }
}
