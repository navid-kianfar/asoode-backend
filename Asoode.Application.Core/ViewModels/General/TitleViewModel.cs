using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.General;

public class TitleViewModel
{
    [Required] public string Title { get; set; }
}