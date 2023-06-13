using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.General.Search;

public class SearchRequestViewModel
{
    [Required] public string Search { get; set; }
}