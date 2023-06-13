using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class PostTaskCommentViewModel
{
    [Required] public string Message { get; set; }
    public bool Private { get; set; }
}