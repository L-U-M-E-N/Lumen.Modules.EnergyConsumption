namespace Lumen.Modules.EnergyConsumption.Common.Models {
    public class EnergyConsumptionPointInTime {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Value { get; set; }
        public string Source { get; set; } = null!;
    }
}
