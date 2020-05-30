namespace PiControlPanel.Infrastructure.Persistence.Services.Cpu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Repositories;

    /// <inheritdoc/>
    public class CpuLoadStatusService :
        BaseTimedService<Domain.Models.Hardware.Cpu.CpuLoadStatus, Entities.Cpu.CpuLoadStatus>,
        ICpuLoadStatusService
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuLoadStatusService"/> class.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuLoadStatusService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.configuration = configuration;
            this.repository = unitOfWork.CpuLoadStatusRepository;
        }

        /// <summary>
        ///     Gets the RealTimeLoads from the list of datetimes.
        /// </summary>
        /// <remarks>
        ///     The unit of work is being created manually instead of relying on the dependency injection
        ///     lifetime due to existing issues with GraphQL dataloader.
        ///     https://github.com/graphql-dotnet/graphql-dotnet/pull/1511
        ///     https://github.com/graphql-dotnet/graphql-dotnet/issues/1310
        ///     Once they are addressed, this can be changed to get the unit of work directly in the
        ///     constructor via dependency injection.
        /// </remarks>
        /// <param name="dateTimes">List of datetimes for which to fetch the RealTimeLoads.</param>
        /// <returns>A dictionary containg the datetimes as keys and the RealTimeLoads as values.</returns>
        public async Task<IDictionary<DateTime, Domain.Models.Hardware.Cpu.CpuLoadStatus>> GetCpuLoadStatusesAsync(
            IEnumerable<DateTime> dateTimes)
        {
            using (var unitOfWork = new UnitOfWork(this.configuration, this.logger))
            {
                var repository = unitOfWork.CpuLoadStatusRepository;
                var entities = await repository.GetMany(l => dateTimes.Contains(l.DateTime))
                    .ToDictionaryAsync(i => i.DateTime, i => i);
                return this.mapper.Map<Dictionary<DateTime, Domain.Models.Hardware.Cpu.CpuLoadStatus>>(entities);
            }
        }

        /// <inheritdoc/>
        protected override IQueryable<Entities.Cpu.CpuLoadStatus> GetAll(LambdaExpression where = null)
        {
            return base.GetAll().Include(s => s.CpuProcesses);
        }
    }
}
