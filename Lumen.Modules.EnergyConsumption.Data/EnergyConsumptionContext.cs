using Lumen.Modules.EnergyConsumption.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.EnergyConsumption.Data {
    public class EnergyConsumptionContext : DbContext {
        public const string SCHEMA_NAME = "EnergyConsumption";

        public EnergyConsumptionContext(DbContextOptions<EnergyConsumptionContext> options) : base(options) {
        }

        public DbSet<EnergyConsumptionPointInTime> EnergyConsumption { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var EnergyConsumptionModelBuilder = modelBuilder.Entity<EnergyConsumptionPointInTime>();
            EnergyConsumptionModelBuilder.Property(x => x.Time)
                .HasColumnType("timestamp with time zone");

            EnergyConsumptionModelBuilder.Property(x => x.Value)
                .HasColumnType("integer");

            EnergyConsumptionModelBuilder.HasKey(x => x.Time);
        }
    }
}
