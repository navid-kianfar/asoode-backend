using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class EditAdvancedCommentViewModel
{
    public float StartFrame { get; set; }
    public float? EndFrame { get; set; }
    [MaxLength(1000)] public string Message { get; set; }
}