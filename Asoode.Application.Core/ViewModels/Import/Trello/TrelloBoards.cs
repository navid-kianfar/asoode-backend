using Newtonsoft.Json;

namespace Asoode.Application.Core.ViewModels.Import.Trello;

public class TrelloBoards
{
    [JsonProperty("totalMembersPerBoard")] public TrelloPerBoard TotalMembersPerBoard { get; set; }
}