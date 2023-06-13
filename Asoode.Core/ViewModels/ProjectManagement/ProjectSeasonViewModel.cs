using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class ProjectSeasonViewModel : BaseViewModel
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}