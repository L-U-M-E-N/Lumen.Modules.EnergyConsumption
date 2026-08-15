using Lumen.Modules.Sdk;
using Lumen.Modules.EnergyConsumption.Data;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.EnergyConsumption.Module {
    public class EnergyConsumptionModule(IEnumerable<ConfigEntry> configEntries, ILogger<LumenModuleBase> logger, IServiceProvider provider) : LumenModuleBase(configEntries, logger, provider) {
        public override Task InitAsync(LumenModuleRunsOnFlag currentEnv) {
            // Nothing to do
            return Task.CompletedTask;
        }

        public override async Task RunAsync(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            try {
                logger.LogTrace($"[{nameof(EnergyConsumptionModule)}] Running tasks ...");
                throw new NotImplementedException();
            } catch (Exception ex) {
                logger.LogError(ex, $"[{nameof(EnergyConsumptionModule)}] Error when running tasks.");
            }
        }

        public override bool ShouldRunNow(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            return currentEnv switch {
                _ => false,
            };
        }

        public override Task ShutdownAsync() {
            // Nothing to do
            return Task.CompletedTask;
        }

        public static new void SetupServices(LumenModuleRunsOnFlag currentEnv, IServiceCollection serviceCollection, string? postgresConnectionString) {
            if (currentEnv == LumenModuleRunsOnFlag.API) {
                serviceCollection.AddDbContext<EnergyConsumptionContext>(o => o.UseNpgsql(postgresConnectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", EnergyConsumptionContext.SCHEMA_NAME)));
            }
        }

        public override Type GetDatabaseContextType() {
            return typeof(EnergyConsumptionContext);
        }
    }
}
