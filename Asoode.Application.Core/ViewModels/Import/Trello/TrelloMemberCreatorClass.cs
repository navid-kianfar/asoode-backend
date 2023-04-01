using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloMemberCreatorClass
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("activityBlocked")] public bool ActivityBlocked { get; set; }

    [JsonProperty("avatarHash")] public string AvatarHash { get; set; }

    [JsonProperty("avatarUrl")] public object AvatarUrl { get; set; }

    [JsonProperty("fullName")] public string FullName { get; set; }

    [JsonProperty("idMemberReferrer")] public object IdMemberReferrer { get; set; }

    [JsonProperty("initials")] public string Initials { get; set; }

    [JsonProperty("nonPublic")] public TrelloNonPublic NonPublic { get; set; }

    [JsonProperty("nonPublicAvailable")] public bool NonPublicAvailable { get; set; }

    [JsonProperty("username")] public string Username { get; set; }
}