using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class AdvancedPlayerCommentViewModel : BaseViewModel
{
    public Guid AttachmentId { get; set; }

    public float StartFrame { get; set; }
    public float? EndFrame { get; set; }
    [MaxLength(1000)] public string Message { get; set; }
    [MaxLength(1000)] public string Payload { get; set; }
}