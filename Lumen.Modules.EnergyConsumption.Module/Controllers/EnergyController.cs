using Lumen.Modules.EnergyConsumption.Common.Models;
using Lumen.Modules.EnergyConsumption.Data;

using Microsoft.AspNetCore.Mvc;

namespace Lumen.Modules.EnergyConsumption.Module.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class EnergyController(ILogger<EnergyController> logger, EnergyConsumptionContext context) : ControllerBase {

        [HttpPost]
        public async Task<IActionResult> SubmitEtnries([FromBody] IEnumerable<EnergyConsumptionPointInTime> entries, CancellationToken cancellationToken = default) {
            if (entries.Count() > 100) {
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
                if (context.EnergyConsumption.Any((x) => x.From == newEntry.From && x.To == newEntry.To && x.Source == newEntry.Source)) {
                    logger.LogInformation("{Module} - Entry '{From}' '{To}' '{Source}' already existing, skipping", nameof(EnergyConsumptionModule), newEntry.From, newEntry.To, newEntry.Source);
                    continue;
                }

                await context.EnergyConsumption.AddAsync(newEntry, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
