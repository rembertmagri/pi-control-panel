namespace PiControlPanel.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ControlPanelDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public ControlPanelDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            Database.EnsureCreated();
        }

        public DbSet<Entities.Chipset> Chipset { get; set; }

        public DbSet<Entities.Cpu.Cpu> Cpu { get; set; }

        public DbSet<Entities.Cpu.CpuFrequency> CpuFrequency { get; set; }

        public DbSet<Entities.Cpu.CpuTemperature> CpuTemperature { get; set; }

        public DbSet<Entities.Cpu.CpuLoadStatus> CpuLoadStatus { get; set; }

        public DbSet<Entities.Cpu.CpuProcess> CpuProcess { get; set; }

        public DbSet<Entities.Gpu> Gpu { get; set; }

        public DbSet<Entities.Os.Os> Os { get; set; }

        public DbSet<Entities.Os.OsStatus> OsStatus { get; set; }

        public DbSet<Entities.Disk.Disk> Disk { get; set; }

        public DbSet<Entities.Disk.DiskStatus> DiskStatus { get; set; }

        public DbSet<Entities.Memory.RandomAccessMemory> RandomAccessMemory { get; set; }

        public DbSet<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatus { get; set; }

        public DbSet<Entities.Memory.SwapMemory> SwapMemory { get; set; }

        public DbSet<Entities.Memory.SwapMemoryStatus> SwapMemoryStatus { get; set; }

        public DbSet<Entities.Network.Network> Network { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
