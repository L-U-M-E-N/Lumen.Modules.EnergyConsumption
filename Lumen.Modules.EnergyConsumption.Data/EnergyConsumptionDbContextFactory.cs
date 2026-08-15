using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lumen.Modules.EnergyConsumption.Data {
    public class EnergyConsumptionDbContextFactory : IDesignTimeDbContextFactory<EnergyConsumptionContext> {
        public EnergyConsumptionContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<EnergyConsumptionContext>();
            optionsBuilder.UseNpgsql();

            return new EnergyConsumptionContext(optionsBuilder.Options);
        }
    }
}
