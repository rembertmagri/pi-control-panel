namespace PiControlPanel.Application.Services
{
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using LightInject;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Repositories;
    using AutoMapper;
    using PiControlPanel.Infrastructure.Persistence.MapperProfile;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using Contracts = PiControlPanel.Domain.Contracts.Infrastructure;
    using OnDemand = PiControlPanel.Infrastructure.OnDemand.Services;
    using Persistence = PiControlPanel.Infrastructure.Persistence.Services;

    /// <summary>
    ///     Implementation of LightInject's ICompositionRoot responsible for
    ///     registering all services required for the Application layer.
    /// </summary>
    public class ApplicationCompositionRoot : ICompositionRoot
    {
        /// <summary>
        ///     Called after LightInject ServiceContainer RegisterFor method is called.
        /// </summary>
        /// <param name="serviceRegistry">LightInject's service registry.</param>
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IUnitOfWork, UnitOfWork>(new PerRequestLifeTime());

            serviceRegistry.Register<Contracts.Persistence.IChipsetService, Persistence.ChipsetService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuService, Persistence.Cpu.CpuService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuFrequencyService, Persistence.Cpu.CpuFrequencyService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuTemperatureService, Persistence.Cpu.CpuTemperatureService>();
            serviceRegistry.Register<Contracts.Persistence.Cpu.ICpuLoadStatusService, Persistence.Cpu.CpuLoadStatusService>();
            serviceRegistry.Register<Contracts.Persistence.IGpuService, Persistence.GpuService>();
            serviceRegistry.Register<Contracts.Persistence.Os.IOsService, Persistence.Os.OsService>();
            serviceRegistry.Register<Contracts.Persistence.Os.IOsStatusService, Persistence.Os.OsStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Disk.IDiskService, Persistence.Disk.DiskService>();
            serviceRegistry.Register<Contracts.Persistence.Disk.IFileSystemStatusService, Persistence.Disk.FileSystemStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryService<RandomAccessMemory>,
                Persistence.Memory.RandomAccessMemoryService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryStatusService<RandomAccessMemoryStatus>,
                Persistence.Memory.RandomAccessMemoryStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryService<SwapMemory>,
                Persistence.Memory.SwapMemoryService>();
            serviceRegistry.Register<Contracts.Persistence.Memory.IMemoryStatusService<SwapMemoryStatus>,
                Persistence.Memory.SwapMemoryStatusService>();
            serviceRegistry.Register<Contracts.Persistence.Network.INetworkService, Persistence.Network.NetworkService>();
            serviceRegistry.Register<Contracts.Persistence.Network.INetworkInterfaceStatusService, Persistence.Network.NetworkInterfaceStatusService>();

            serviceRegistry.Register<Contracts.OnDemand.IControlPanelService, OnDemand.ControlPanelService>();
            serviceRegistry.Register<Contracts.OnDemand.IUserAccountService, OnDemand.UserAccountService>();
            serviceRegistry.Register<Contracts.OnDemand.IChipsetService, OnDemand.ChipsetService>();
            serviceRegistry.Register<Contracts.OnDemand.ICpuService, OnDemand.CpuService>();
            serviceRegistry.Register<Contracts.OnDemand.IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus>,
                OnDemand.MemoryService<RandomAccessMemory, RandomAccessMemoryStatus>>();
            serviceRegistry.Register<Contracts.OnDemand.IMemoryService<SwapMemory, SwapMemoryStatus>,
                OnDemand.MemoryService<SwapMemory, SwapMemoryStatus>>();
            serviceRegistry.Register<Contracts.OnDemand.IGpuService, OnDemand.GpuService>();
            serviceRegistry.Register<Contracts.OnDemand.IDiskService, OnDemand.DiskService>();
            serviceRegistry.Register<Contracts.OnDemand.IOsService, OnDemand.OsService>();
            serviceRegistry.Register<Contracts.OnDemand.INetworkService, OnDemand.NetworkService>();

            serviceRegistry.RegisterSingleton<IMapper>(factory => new AutoMapperConfiguration().GetIMapper());

            serviceRegistry.RegisterSingleton<ISubject<CpuFrequency>>(factory => new ReplaySubject<CpuFrequency>(1));
            serviceRegistry.RegisterSingleton<ISubject<CpuTemperature>>(factory => new ReplaySubject<CpuTemperature>(1));
            serviceRegistry.RegisterSingleton<ISubject<CpuLoadStatus>>(factory => new ReplaySubject<CpuLoadStatus>(1));
            serviceRegistry.RegisterSingleton<ISubject<RandomAccessMemoryStatus>>(factory => new ReplaySubject<RandomAccessMemoryStatus>(1));
            serviceRegistry.RegisterSingleton<ISubject<SwapMemoryStatus>>(factory => new ReplaySubject<SwapMemoryStatus>(1));
            serviceRegistry.RegisterSingleton<ISubject<IList<FileSystemStatus>>>(factory => new ReplaySubject<IList<FileSystemStatus>>(1));
            serviceRegistry.RegisterSingleton<ISubject<OsStatus>>(factory => new ReplaySubject<OsStatus>(1));
            serviceRegistry.RegisterSingleton<ISubject<IList<NetworkInterfaceStatus>>>(factory => new ReplaySubject<IList<NetworkInterfaceStatus>>(1));
        }
    }
}
