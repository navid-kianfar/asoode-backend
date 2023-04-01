using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class SubProjectViewModel : BaseViewModel
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
}