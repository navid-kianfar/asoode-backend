using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class CreateSubProjectViewModel : SimpleViewModel
{
    public Guid? ParentId { get; set; }
}