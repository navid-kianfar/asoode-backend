using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class MoveTaskViewModel : RepositionViewModel
{
    [Required] public Guid ListId { get; set; }
}