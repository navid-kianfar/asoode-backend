using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackageCustomFieldViewModel
{
    public Guid PackageId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SubProjectId { get; set; }
    public bool Show { get; set; }
    public string Title { get; set; }
    public WorkPackageCustomFieldType Type { get; set; }
}