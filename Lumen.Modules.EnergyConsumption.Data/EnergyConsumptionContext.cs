using Lumen.Modules.EnergyConsumption.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.EnergyConsumption.Data {
    public class EnergyConsumptionContext(DbContextOptions<EnergyConsumptionContext> options) : DbContext(options) {
        public const string SCHEMA_NAME = "EnergyConsumption";

        public DbSet<EnergyConsumptionPointInTime> EnergyConsumption { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var EnergyConsumptionModelBuilder = modelBuilder.Entity<EnergyConsumptionPointInTime>();
            EnergyConsumptionModelBuilder.Property(x => x.From)
                .HasColumnType("timestamp with time zone");
            EnergyConsumptionModelBuilder.Property(x => x.To)
                .HasColumnType("timestamp with time zone");

            EnergyConsumptionModelBuilder.Property(x => x.Value)
                .HasColumnType("integer");

            EnergyConsumptionModelBuilder.Property(x => x.Source)
                .IsUnicode(true)
                .HasMaxLength(1000);

            EnergyConsumptionModelBuilder.HasKey(x => new { x.From, x.To, x.Source });
        }
    }
}
