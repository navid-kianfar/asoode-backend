using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloWelcomePrefs
{
    [JsonProperty("permissionLevel")] public string PermissionLevel { get; set; }

    [JsonProperty("hideVotes")] public bool HideVotes { get; set; }

    [JsonProperty("voting")] public string Voting { get; set; }

    [JsonProperty("comments")] public string Comments { get; set; }

    [JsonProperty("invitations")] public string Invitations { get; set; }

    [JsonProperty("selfJoin")] public bool SelfJoin { get; set; }

    [JsonProperty("cardCovers")] public bool CardCovers { get; set; }

    [JsonProperty("isTemplate")] public bool IsTemplate { get; set; }

    [JsonProperty("cardAging")] public string CardAging { get; set; }

    [JsonProperty("calendarFeedEnabled")] public bool CalendarFeedEnabled { get; set; }

    [JsonProperty("background")] public string Background { get; set; }

    [JsonProperty("backgroundImage")] public Uri BackgroundImage { get; set; }

    [JsonProperty("backgroundImageScaled")]
    public TrelloBackgroundImageScaled[] BackgroundImageScaled { get; set; }

    [JsonProperty("backgroundTile")] public bool BackgroundTile { get; set; }

    [JsonProperty("backgroundBrightness")] public string BackgroundBrightness { get; set; }

    [JsonProperty("backgroundBottomColor")]
    public string BackgroundBottomColor { get; set; }

    [JsonProperty("backgroundTopColor")] public string BackgroundTopColor { get; set; }

    [JsonProperty("canBePublic")] public bool CanBePublic { get; set; }

    [JsonProperty("canBeEnterprise")] public bool CanBeEnterprise { get; set; }

    [JsonProperty("canBeOrg")] public bool CanBeOrg { get; set; }

    [JsonProperty("canBePrivate")] public bool CanBePrivate { get; set; }

    [JsonProperty("canInvite")] public bool CanInvite { get; set; }
}