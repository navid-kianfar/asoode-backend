using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class CreateTaskViewModel : TitleViewModel
{
    [Required] public Guid ListId { get; set; }
    public Guid? ParentId { get; set; }
    public int? Count { get; set; }
}