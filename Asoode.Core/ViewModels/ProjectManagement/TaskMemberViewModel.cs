using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class TaskMemberViewModel
{
    public bool IsGroup { get; set; }
    [Required] public Guid RecordId { get; set; }
}