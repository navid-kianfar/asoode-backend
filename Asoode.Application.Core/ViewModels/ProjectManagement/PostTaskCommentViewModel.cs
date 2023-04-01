using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class PostTaskCommentViewModel
{
    [Required] public string Message { get; set; }
    public bool Private { get; set; }
}