namespace Asoode.Core.ViewModels.ProjectManagement;

public class WorkPackagePermissionViewModel
{
    public bool PermissionComment { get; set; }
    public bool PermissionEditAttachment { get; set; }
    public bool PermissionCreateAttachment { get; set; }
    public bool PermissionAssignMembers { get; set; }
    public bool PermissionAssignLabels { get; set; }
    public bool PermissionChangeTaskState { get; set; }
    public bool PermissionEditTask { get; set; }
    public bool PermissionArchiveTask { get; set; }
    public bool PermissionCreateTask { get; set; }
    public bool PermissionArchiveList { get; set; }
    public bool PermissionEditList { get; set; }
    public bool PermissionCreateList { get; set; }
}