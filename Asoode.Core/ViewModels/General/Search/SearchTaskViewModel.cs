using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.ProjectManagement;

namespace Asoode.Core.ViewModels.General.Search;

public class SearchTaskViewModel : BaseViewModel
{
    public SearchTaskViewModel()
    {
        Members = new MemberInfoViewModel[0];
        Labels = new TaskLabelViewModel[0];
    }

    public WorkPackageTaskState State { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string List { get; set; }
    public string WorkPackage { get; set; }
    public string Project { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid WorkPackageId { get; set; }
    public Guid ProjectId { get; set; }
    public TaskLabelViewModel[] Labels { get; set; }
    public MemberInfoViewModel[] Members { get; set; }
}