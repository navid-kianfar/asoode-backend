using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class CreateSubProjectViewModel : SimpleViewModel
{
    public Guid? ParentId { get; set; }
}