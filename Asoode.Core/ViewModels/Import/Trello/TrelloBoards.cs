using Newtonsoft.Json;

namespace Asoode.Core.ViewModels.Import.Trello;

public class TrelloBoards
{
    [JsonProperty("totalMembersPerBoard")] public TrelloPerBoard TotalMembersPerBoard { get; set; }
}