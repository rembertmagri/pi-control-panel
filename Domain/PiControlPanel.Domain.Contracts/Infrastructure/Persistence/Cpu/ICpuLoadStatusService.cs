namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICpuLoadStatusService : IBaseTimedObjectService<CpuLoadStatus>
    {
        Task<IDictionary<DateTime, CpuLoadStatus>> GetCpuLoadStatusesAsync(
            IEnumerable<DateTime> dateTimes);
    }
}
