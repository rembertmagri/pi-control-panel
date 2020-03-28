namespace PiControlPanel.Application.Services
{
    using System.Reactive.Subjects;
    using LightInject;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Repositories;
    using AutoMapper;
    using PiControlPanel.Infrastructure.Persistence.MapperProfile;
    using Contracts = PiControlPanel.Domain.Contracts.Infrastructure;
    using Persistence = PiControlPanel.Infrastructure.Persistence.Services;
    using OnDemand = PiControlPanel.Infrastructure.OnDemand.Services;

    /// <summary>
    ///     Implementation of LightInject's ICompositionRoot responsible for
    ///     registering all services required for the Application layer
    /// </summary>
    public class AppCompositionRoot : ICompositionRoot
    {
        /// <summary>
        ///     Called after LightInject ServiceContainer RegisterFor method is called
        /// </summary>
        /// <param name="serviceRegistry">LightInject's service registry</param>
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IUnitOfWork, UnitOfWork>(new PerRequestLifeTime());
            serviceRegistry.Register<Contracts.Persistence.IChipsetService, Persistence.ChipsetService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuService, Persistence.Cpu.CpuService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuTemperatureService, Persistence.Cpu.CpuTemperatureService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuAverageLoadService, Persistence.Cpu.CpuAverageLoadService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuRealTimeLoadService, Persistence.Cpu.CpuRealTimeLoadService>();
            serviceRegistry.Register<Contracts.Persistence.IGpuService, Persistence.GpuService>();
            serviceRegistry.Register<Contracts.Persistence.IOsService, Persistence.OsService>();
            serviceRegistry.Register<Contracts.Persistence.Disk.IDiskService, Persistence.Disk.DiskService>();
            serviceRegistry.Register<Contracts.Persistence.Disk.IDiskStatusService, Persistence.Disk.DiskStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryService<RandomAccessMemory>,
                Persistence.Memory.RandomAccessMemoryService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryStatusService<RandomAccessMemoryStatus>,
                Persistence.Memory.RandomAccessMemoryStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryService<SwapMemory>,
                Persistence.Memory.SwapMemoryService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryStatusService<SwapMemoryStatus>,
                Persistence.Memory.SwapMemoryStatusService>();

            serviceRegistry.Register<Contracts.OnDemand.IControlPanelService, OnDemand.ControlPanelService>();
            serviceRegistry.Register<Contracts.OnDemand.IChipsetService, OnDemand.ChipsetService>();
            serviceRegistry.Register<Contracts.OnDemand.ICpuService, OnDemand.CpuService>();
            serviceRegistry.Register<Contracts.OnDemand.IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus>,
                OnDemand.MemoryService<RandomAccessMemory, RandomAccessMemoryStatus>>();
            serviceRegistry.Register<Contracts.OnDemand.IMemoryService<SwapMemory, SwapMemoryStatus>,
                OnDemand.MemoryService<SwapMemory, SwapMemoryStatus>>();
            serviceRegistry.Register<Contracts.OnDemand.IGpuService, OnDemand.GpuService>();
            serviceRegistry.Register<Contracts.OnDemand.IDiskService, OnDemand.DiskService>();
            serviceRegistry.Register<Contracts.OnDemand.IOsService, OnDemand.OsService>();
            serviceRegistry.Register<Contracts.OnDemand.IUserAccountService, OnDemand.UserAccountService>();
            
            serviceRegistry.RegisterSingleton<IMapper>(factory => new AutoMapperConfiguration().GetIMapper());

            serviceRegistry.RegisterSingleton<ISubject<CpuTemperature>>(factory => new ReplaySubject<CpuTemperature>(1));
            serviceRegistry.RegisterSingleton<ISubject<CpuAverageLoad>>(factory => new ReplaySubject<CpuAverageLoad>(1));
            serviceRegistry.RegisterSingleton<ISubject<CpuRealTimeLoad>>(factory => new ReplaySubject<CpuRealTimeLoad>(1));
            serviceRegistry.RegisterSingleton<ISubject<MemoryStatus>>(factory => new ReplaySubject<MemoryStatus>(1));
            serviceRegistry.RegisterSingleton<ISubject<DiskStatus>>(factory => new ReplaySubject<DiskStatus>(1));
        }
    }
}
