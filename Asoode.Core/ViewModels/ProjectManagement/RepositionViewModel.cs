using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class RepositionViewModel
{
    [Required] public int Order { get; set; }
}