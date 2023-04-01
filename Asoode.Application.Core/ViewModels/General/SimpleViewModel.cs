using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.General;

public class SimpleViewModel
{
    [Required] public string Title { get; set; }
    public string Description { get; set; }
}