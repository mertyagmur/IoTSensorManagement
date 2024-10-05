using IoTSensorManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace IoTSensorManagement.Core.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Device> Devices { get; set; }
		public DbSet<SensorData> SensorData { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Device>()
				.HasKey(d => d.Id);

			modelBuilder.Entity<Device>()
				.HasMany(d => d.SensorData)
				.WithOne()
				.HasForeignKey(sd => sd.DeviceId);

			modelBuilder.Entity<SensorData>()
				.HasKey(sd => sd.Id);

			modelBuilder.Entity<SensorData>()
				.Property(sd => sd.Value)
				.HasPrecision(18, 2);
		}
	}
}
