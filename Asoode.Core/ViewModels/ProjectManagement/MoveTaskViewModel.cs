using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class MoveTaskViewModel : RepositionViewModel
{
    [Required] public Guid ListId { get; set; }
}