using System.Text.Json.Serialization;

namespace PressMachineMainModeules.Models
{
    public class PressMachineHistoryParams
    {
        [JsonPropertyName("ParasName")] public string ParasName { get; set; }
        [JsonPropertyName("ParasWriteIndex")] public bool ParasWriteIndex { get; set; }
        [JsonPropertyName("XUnit")] public string XUnit { get; set; }
        [JsonPropertyName("YUnit")] public string YUnit { get; set; }
        [JsonPropertyName("XName")] public string XName { get; set; }
        [JsonPropertyName("YName")] public string YName { get; set; }

        [JsonPropertyName("PositionUnitName")] public string PositionUnitName { get; set; }
        [JsonPropertyName("PressUnitName")] public string PressUnitName { get; set; }
        [JsonPropertyName("SpeedUnitName")] public string SpeedUnitName { get; set; }
    }
}
