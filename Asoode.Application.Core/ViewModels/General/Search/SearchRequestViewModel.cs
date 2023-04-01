using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.General.Search;

public class SearchRequestViewModel
{
    [Required] public string Search { get; set; }
}