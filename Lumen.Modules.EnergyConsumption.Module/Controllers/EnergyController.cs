using Lumen.Modules.EnergyConsumption.Common.Models;
using Lumen.Modules.EnergyConsumption.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.EnergyConsumption.Module.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class EnergyController(ILogger<EnergyController> logger, EnergyConsumptionContext context) : ControllerBase {

        [HttpPost]
        public async Task<IActionResult> SubmitEtnries([FromBody] IEnumerable<EnergyConsumptionPointInTime> entries, CancellationToken cancellationToken = default) {
            if (entries.Count() > 150) {
                return BadRequest();
            }

            try {
                await UpserEntries(entries, cancellationToken);

                return Ok();
            } catch (Exception ex) {
                logger.LogError(ex, "Unexpected error when adding entries");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error");
            }
        }

        private async Task UpserEntries(IEnumerable<EnergyConsumptionPointInTime> entries, CancellationToken cancellationToken) {
            foreach (var newEntry in entries) {
                var entry = await context.EnergyConsumption.FirstOrDefaultAsync((x) => x.From == newEntry.From && x.To == newEntry.To && x.Source == newEntry.Source, cancellationToken);
                if (entry is not null) {
                    logger.LogInformation("{Module} - Entry '{From}' '{To}' '{Source}' already existing, updating", nameof(EnergyConsumptionModule), newEntry.From, newEntry.To, newEntry.Source);
                    if (newEntry.Value <= entry.Value) {
                        continue;
                    }
                    entry.Value = newEntry.Value;
                    continue;
                }

                logger.LogInformation("{Module} - Entry '{From}' '{To}' '{Source}' not already existing, adding", nameof(EnergyConsumptionModule), newEntry.From, newEntry.To, newEntry.Source);
                await context.EnergyConsumption.AddAsync(newEntry, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
