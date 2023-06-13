using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Taskulu;

public class WeeklyReport
{
    [JsonProperty("enabled")] public bool Enabled { get; set; }

    [JsonProperty("day")] public string Day { get; set; }

    [JsonProperty("team_id")] public string TeamId { get; set; }
}