using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Logging;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Reports;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Asoode.Business.ProjectManagement;

public class TaskBiz : ITaskBiz
{
    private readonly IServiceProvider _serviceProvider;

    public TaskBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #region Create

    public async Task<OperationResult<bool>> Create(Guid userId, Guid packageId, CreateTaskViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.FindUser(userId);
                if (user == null) return OperationResult<bool>.Rejected();
                var packageAccess = await unit.WorkPackageMembers
                    .Where(p =>
                        p.PackageId == packageId && (p.RecordId == userId || p.IsGroup)
                                                 && p.Access != AccessType.Visitor)
                    .AsNoTracking().ToArrayAsync();
                if (!packageAccess.Any()) return OperationResult<bool>.Rejected();
                if (packageAccess.All(p => p.IsGroup))
                {
                    var groupIds = await unit.GroupMembers.Where(i => i.UserId == userId)
                        .Select(i => i.GroupId).ToArrayAsync();
                    if (!packageAccess.Any(a => groupIds.Contains(a.RecordId)))
                        return OperationResult<bool>.Rejected();
                }

                var package = await unit.WorkPackages.AsNoTracking().SingleAsync(p => p.Id == packageId);
                if (package.ArchivedAt.HasValue || package.DeletedAt.HasValue)
                    return OperationResult<bool>.Rejected();

                var taskList = await unit.WorkPackageLists.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == model.ListId);

                if (taskList == null || taskList.ArchivedAt.HasValue || taskList.DeletedAt.HasValue)
                    return OperationResult<bool>.Rejected();

                var counter = 2;
                var existing = await unit.WorkPackageTasks
                    .Where(l => l.ListId == model.ListId && l.ParentId == model.ParentId)
                    .OrderBy(i => i.Order)
                    .ToListAsync();

                if (!model.Count.HasValue || model.Count.Value == 1)
                {
                    var task = new WorkPackageTask
                    {
                        Title = model.Title.Trim(),
                        UserId = userId,
                        State = WorkPackageTaskState.ToDo,
                        ListId = model.ListId,
                        PackageId = packageId,
                        ProjectId = package.ProjectId,
                        SubProjectId = package.SubProjectId,
                        ParentId = model.ParentId,
                        Restricted = taskList.Restricted,
                        VoteNecessity = WorkPackageTaskVoteNecessity.None,
                        Order = 1
                    };
                    var member = new WorkPackageTaskMember
                    {
                        PackageId = packageId,
                        TaskId = task.Id,
                        RecordId = userId
                    };

                    existing.ForEach(e => e.Order = counter++);
                    await unit.WorkPackageTasks.AddAsync(task);
                    await unit.WorkPackageTaskMember.AddAsync(member);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageTaskAdd,
                        UserId = userId,
                        PackageTask = task.ToViewModel(new[] { member.ToViewModel() })
                    });

                    return OperationResult<bool>.Success(true);
                }

                counter = model.Count.Value;
                var tasks = new List<WorkPackageTask>();
                var members = new List<WorkPackageTaskMember>();
                for (var i = 0; i < model.Count.Value; i++)
                {
                    var title = model.Title.Trim()
                        .Replace("$", (i + 1).ToString("D3"));
                    var task = new WorkPackageTask
                    {
                        Title = title,
                        UserId = userId,
                        State = WorkPackageTaskState.ToDo,
                        ListId = model.ListId,
                        PackageId = packageId,
                        ProjectId = package.ProjectId,
                        SubProjectId = package.SubProjectId,
                        ParentId = model.ParentId,
                        Restricted = taskList.Restricted,
                        VoteNecessity = WorkPackageTaskVoteNecessity.None,
                        Order = i + 1
                    };
                    var member = new WorkPackageTaskMember
                    {
                        PackageId = packageId,
                        TaskId = task.Id,
                        RecordId = userId
                    };

                    tasks.Add(task);
                    members.Add(member);
                }

                await unit.WorkPackageTasks.AddRangeAsync(tasks);
                await unit.WorkPackageTaskMember.AddRangeAsync(members);
                existing.ForEach(e => e.Order = counter++);

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskBulkAdd,
                    UserId = userId,
                    WorkPackage = package.ToViewModel(),
                    PackageTasks = tasks.Select(t =>
                    {
                        var m = members.Where(s => s.TaskId == t.Id)
                            .Select(s => s.ToViewModel())
                            .ToArray();
                        return t.ToViewModel(m);
                    }).ToArray(),
                    User = user.ToViewModel()
                });

                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Due / Range

    public async Task<OperationResult<bool>> SetDate(Guid userId, Guid taskId, SetDateViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                if (
                    (model.BeginAt.HasValue && !model.EndAt.HasValue) ||
                    (!model.BeginAt.HasValue && model.EndAt.HasValue)
                ) return OperationResult<bool>.Rejected();

                if (model.BeginAt.HasValue && model.EndAt.HasValue && model.EndAt.Value < model.BeginAt.Value)
                    return OperationResult<bool>.Validation();

                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageAdmin(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                task.UpdatedAt = DateTime.UtcNow;
                task.DueAt = model.DueAt;
                task.BeginAt = model.BeginAt;
                task.EndAt = model.EndAt;
                task.LastDuePassedNotified = null;
                task.LastEndPassedNotified = null;

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Fetch

    public async Task<OperationResult<WorkPackageTaskViewModel>> Detail(Guid userId, Guid taskId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<WorkPackageTaskViewModel>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<WorkPackageTaskViewModel>.Rejected();

                var list = await unit.WorkPackageLists.AsNoTracking()
                    .SingleAsync(i => i.Id == task.ListId);

                // TODO: check if list is private

                var allSubTasks = await unit.WorkPackageTasks
                    .Where(l => l.ParentId == taskId &&
                                (
                                    task.ArchivedAt.HasValue ||
                                    (!task.ArchivedAt.HasValue && !l.ArchivedAt.HasValue)
                                )
                    )
                    .OrderByDescending(i => i.Order)
                    .ToArrayAsync();

                var allSubTaskIds = allSubTasks.Select(i => i.Id).ToArray();

                var workPackageTaskMembers = await unit.WorkPackageTaskMember
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .ToArrayAsync();

                var workPackageTaskLabels = await unit.WorkPackageTaskLabels
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var workPackageTaskComments = await unit.WorkPackageTaskComments
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var workPackageTaskObjective = await unit.WorkPackageTaskObjectives
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

                var workPackageTaskAttachments = await unit.WorkPackageTaskAttachments
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var workPackageTaskCustomFields = await unit.WorkPackageTaskCustomFields
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var workPackageTaskVotes = await unit.WorkPackageTaskVotes
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var interaction = await unit.WorkPackageTaskInteractions
                    .Where(l => l.TaskId == taskId && l.UserId == userId && !l.DeletedAt.HasValue)
                    .Select(i => i.Watching)
                    .SingleOrDefaultAsync();

                var workPackageTaskTimeSpent = await unit.WorkPackageTaskTimes
                    .Where(l => (l.TaskId == taskId || allSubTaskIds.Contains(l.TaskId)) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var result = task.ToViewModel(
                    workPackageTaskMembers.Where(i => i.TaskId == taskId).Select(i => i.ToViewModel()).ToArray(),
                    workPackageTaskLabels.Where(i => i.TaskId == taskId).Select(i => i.ToViewModel()).ToArray(),
                    workPackageTaskComments.Where(i => i.TaskId == taskId).Select(i => i.ToViewModel()).ToArray()
                );
                result.ListName = list.Title;
                result.Watching = interaction ?? false;
                result.ObjectiveValue = workPackageTaskObjective?.ObjectiveValue;
                result.Votes = workPackageTaskVotes.Where(i => i.TaskId == taskId).Select(a => a.ToViewModel())
                    .ToArray();
                result.TimeSpents = workPackageTaskTimeSpent.Where(i => i.TaskId == taskId)
                    .Select(a => a.ToViewModel()).ToArray();
                result.Attachments = workPackageTaskAttachments.Where(i => i.TaskId == taskId)
                    .Select(a => a.ToViewModel()).ToArray();
                result.SubTasks = allSubTasks.Select(s =>
                {
                    var tmp = s.ToViewModel(
                        workPackageTaskMembers.Where(i => i.TaskId == s.Id).Select(i => i.ToViewModel()).ToArray(),
                        workPackageTaskLabels.Where(i => i.TaskId == s.Id).Select(i => i.ToViewModel()).ToArray(),
                        workPackageTaskComments.Where(i => i.TaskId == s.Id).Select(i => i.ToViewModel()).ToArray()
                    );
                    tmp.ListName = list.Title;
                    tmp.Votes = workPackageTaskVotes.Where(i => i.TaskId == s.Id).Select(a => a.ToViewModel())
                        .ToArray();
                    tmp.TimeSpents = workPackageTaskTimeSpent.Where(i => i.TaskId == s.Id)
                        .Select(a => a.ToViewModel()).ToArray();
                    tmp.Attachments = workPackageTaskAttachments.Where(i => i.TaskId == s.Id)
                        .Select(a => a.ToViewModel()).ToArray();
                    return tmp;
                }).ToArray();

                return OperationResult<WorkPackageTaskViewModel>.Success(result);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<WorkPackageTaskViewModel>.Failed();
        }
    }

    public async Task<OperationResult<TaskLogViewModel[]>> Logs(Guid userId, Guid taskId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.Blocked || user.IsLocked)
                    return OperationResult<TaskLogViewModel[]>.Rejected();

                var task = await unit.WorkPackageTasks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);
                if (task == null) return OperationResult<TaskLogViewModel[]>.NotFound();

                var groupIds = await unit.GroupMembers
                    .Where(i => i.UserId == userId)
                    .Select(i => i.GroupId)
                    .ToArrayAsync();

                var access = await unit.WorkPackageMembers.Where(i =>
                        i.PackageId == task.PackageId &&
                        (i.RecordId == userId || groupIds.Contains(i.RecordId)))
                    .AnyAsync();
                if (!access) return OperationResult<TaskLogViewModel[]>.Rejected();
                var logs = (await unit.Activities.Where(i =>
                        i.RecordId == taskId).AsNoTracking().ToListAsync())
                    .Select(i => i.ToViewModel()).ToArray();
                return OperationResult<TaskLogViewModel[]>.Success(logs);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<TaskLogViewModel[]>.Failed();
        }
    }

    public async Task<OperationResult<bool>> SpendTime(Guid userId, Guid taskId, DurationViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                WorkPackageTaskTimeViewModel viewModel = null;
                var working = await unit.WorkPackageTaskTimes
                    .SingleOrDefaultAsync(i =>
                        i.UserId == userId &&
                        !i.End.HasValue
                    );

                if (working != null) working.End = DateTime.UtcNow;
                if (working == null || working.TaskId != taskId)
                {
                    working = new WorkPackageTaskTime
                    {
                        Begin = DateTime.UtcNow,
                        PackageId = task.PackageId,
                        ProjectId = task.ProjectId,
                        TaskId = taskId,
                        UserId = userId,
                        SubProjectId = task.SubProjectId
                    };
                    await unit.WorkPackageTaskTimes.AddAsync(working);
                }

                viewModel = working.ToViewModel();

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskTime,
                    UserId = userId,
                    PackageTaskTime = viewModel,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<WorkPackageTaskViewModel[]>> Calendar(Guid userId, DurationViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.IsLocked || user.Blocked)
                    return OperationResult<WorkPackageTaskViewModel[]>.Rejected();

                var allGroupIds = await unit.GroupMembers
                    .Where(i => i.UserId == userId)
                    .Select(i => i.GroupId)
                    .ToArrayAsync();

                var tasks = await (
                    from task in unit.WorkPackageTasks
                    join member in unit.WorkPackageTaskMember on task.Id equals member.TaskId
                    where
                        (member.RecordId == userId || allGroupIds.Contains(member.RecordId)) &&
                        !task.ArchivedAt.HasValue && !task.DeletedAt.HasValue &&
                        task.State != WorkPackageTaskState.Canceled &&
                        task.State != WorkPackageTaskState.Done &&
                        task.State != WorkPackageTaskState.Duplicate &&
                        (
                            (task.DueAt.HasValue && task.DueAt >= model.Begin && task.DueAt <= model.End) ||
                            (task.BeginAt.HasValue && task.BeginAt >= model.Begin && task.BeginAt <= model.End) ||
                            (task.EndAt.HasValue && task.EndAt >= model.Begin && task.EndAt <= model.End)
                        )
                    orderby task.DueAt, task.BeginAt
                    select task
                ).AsNoTracking().ToArrayAsync();

                var data = tasks.Select(t => t.ToViewModel()).ToArray();
                return OperationResult<WorkPackageTaskViewModel[]>.Success(data);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<WorkPackageTaskViewModel[]>.Failed();
        }
    }

    public async Task<OperationResult<KartablViewModel>> Kartabl(Guid userId, DurationViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.IsLocked || user.Blocked)
                    return OperationResult<KartablViewModel>.Rejected();

                var allGroupIds = await unit.GroupMembers
                    .Where(i => i.UserId == userId && i.DeletedAt == null)
                    .Select(i => i.GroupId)
                    .ToArrayAsync();

                var projectIds = await unit.ProjectMembers
                    .Where(i => i.DeletedAt == null &&
                                (i.RecordId == userId || allGroupIds.Contains(i.RecordId))
                    )
                    .Select(i => i.ProjectId)
                    .Distinct()
                    .ToArrayAsync();

                var data = (await (
                    from member in unit.WorkPackageTaskMember
                    join task in unit.WorkPackageTasks on member.TaskId equals task.Id
                    join list in unit.WorkPackageLists on task.ListId equals list.Id
                    where
                        (member.RecordId == userId || allGroupIds.Contains(member.RecordId)) &&
                        projectIds.Contains(task.ProjectId) &&
                        !task.ArchivedAt.HasValue &&
                        !task.DeletedAt.HasValue &&
                        !(
                            task.State == WorkPackageTaskState.Canceled ||
                            task.State == WorkPackageTaskState.Done ||
                            task.State == WorkPackageTaskState.Duplicate
                        )
                    orderby task.DueAt
                    select new { Task = task, List = list.Title }
                ).AsNoTracking().ToArrayAsync()).Select(i =>
                {
                    var vm = i.Task.ToViewModel();
                    vm.ListName = i.List;
                    return vm;
                }).ToArray();

                return OperationResult<KartablViewModel>.Success(new KartablViewModel
                {
                    Tasks = data
                });
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<KartablViewModel>.Failed();
        }
    }

    #endregion

    #region Edit

    public async Task<OperationResult<bool>> Comment(Guid userId, Guid taskId, PostTaskCommentViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var comment = new WorkPackageTaskComment
                {
                    Message = model.Message,
                    Private = model.Private,
                    TaskId = task.Id,
                    UserId = userId,
                    PackageId = task.PackageId,
                    ProjectId = task.ProjectId
                };
                await unit.WorkPackageTaskComments.AddAsync(comment);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskComment,
                    UserId = userId,
                    PackageTaskComment = comment.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Archive(Guid userId, Guid taskId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                if (task.ArchivedAt.HasValue) task.ArchivedAt = null;
                else task.ArchivedAt = DateTime.UtcNow;

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskArchive,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Watch(Guid userId, Guid taskId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.Blocked || user.IsLocked)
                    return OperationResult<bool>.Rejected();

                var task = await unit.WorkPackageTasks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);
                if (task == null) return OperationResult<bool>.NotFound();

                var groupIds = await unit.GroupMembers
                    .Where(i => i.UserId == userId)
                    .Select(i => i.GroupId)
                    .ToArrayAsync();

                var access = await unit.WorkPackageMembers.Where(i =>
                        i.PackageId == task.PackageId &&
                        (i.RecordId == userId || groupIds.Contains(i.RecordId)))
                    .AnyAsync();
                if (!access) return OperationResult<bool>.Rejected();

                var interaction = await unit.WorkPackageTaskInteractions.SingleOrDefaultAsync(
                    i => i.TaskId == taskId && i.UserId == userId);

                if (interaction == null)
                {
                    interaction = new WorkPackageTaskInteraction
                    {
                        Watching = true,
                        UpdatedAt = DateTime.UtcNow,
                        LastView = DateTime.UtcNow,
                        PackageId = task.PackageId,
                        TaskId = taskId,
                        UserId = userId
                    };
                    await unit.WorkPackageTaskInteractions.AddAsync(interaction);
                }
                else
                {
                    interaction.Watching = !interaction.Watching;
                    interaction.UpdatedAt = DateTime.UtcNow;
                }

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskWatch,
                    UserId = userId,
                    PackageTaskInteraction = interaction.ToViewModel(),
                    User = user.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Location(Guid userId, Guid taskId, LocationViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                task.GeoLocation = model.Location?.Trim();
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Estimated(Guid userId, Guid taskId, EstimatedTimeViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageAdmin(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                task.EstimatedTicks = model.Duration;
                task.EstimatedTime = null;

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Reposition(Guid userId, Guid taskId, RepositionViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();
                if (model.Order == task.Order) return OperationResult<bool>.Success(true);

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var tasks = await unit.WorkPackageTasks
                    .Where(i => i.ListId == task.ListId && i.ParentId == task.ParentId)
                    .OrderBy(i => i.Order)
                    .ToListAsync();

                var element = tasks.Single(e => e.Id == taskId);
                tasks.Remove(element);

                // TODO: BUG**
                tasks.Insert(model.Order - 1, element);

                var orders = 1;
                tasks.ForEach(e => { e.Order = orders++; });

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskReposition,
                    UserId = userId,
                    PackageTask = tasks.Find(i => i.Id == taskId).ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Move(Guid userId, Guid taskId, MoveTaskViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();
                if (model.ListId == task.ListId) return OperationResult<bool>.Success(true);

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var destinationListPackage = await unit.WorkPackageLists
                    .Where(i => i.Id == model.ListId && i.PackageId == task.PackageId)
                    .AnyAsync();

                if (!destinationListPackage) return OperationResult<bool>.Rejected();
                var allTasks = await unit.WorkPackageTasks
                    .Where(i => i.ListId == task.ListId || i.ListId == model.ListId)
                    .ToArrayAsync();

                var source = allTasks.Where(t => t.ListId == task.ListId)
                    .OrderBy(i => i.Order).ToList();
                var destination = allTasks.Except(source).OrderBy(i => i.Order).ToList();
                var pop = source.Single(i => i.Id == taskId);
                pop.ListId = model.ListId;

                // TODO: if list is private make task private

                source.Remove(pop);
                destination.Insert(model.Order - 1, pop);
                var counter = 1;
                source.ForEach(s => s.Order = counter++);
                counter = 1;
                destination.ForEach(s => s.Order = counter++);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskMove,
                    UserId = userId,
                    PackageTask = pop.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }


    public async Task<OperationResult<bool>> ChangeTitle(Guid userId, Guid taskId, TitleViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                task.Title = model.Title.Trim();
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ChangeDescription(Guid userId, Guid taskId, TitleViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                task.Description = model.Title.Trim();
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ChangeState(Guid userId, Guid taskId, StateViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                if (task.State == model.State) return OperationResult<bool>.Success();

                var type = ActivityType.WorkPackageTaskEdit;
                var oldState = task.State;
                task.State = model.State;
                switch (task.State)
                {
                    case WorkPackageTaskState.Done:
                        type = ActivityType.WorkPackageTaskDone;
                        task.DoneAt = DateTime.UtcNow;
                        task.DoneUserId = userId;

                        if (oldState == WorkPackageTaskState.Blocker) type = ActivityType.WorkPackageTaskUnBlock;
                        // TODO: handle blocker
                        break;
                    case WorkPackageTaskState.Blocked:
                        type = ActivityType.WorkPackageTaskBlocked;
                        break;
                    case WorkPackageTaskState.Blocker:
                        type = ActivityType.WorkPackageTaskBlocker;
                        break;
                    case WorkPackageTaskState.Paused:
                        type = ActivityType.WorkPackageTaskPaused;
                        break;
                }

                if (oldState == WorkPackageTaskState.Done)
                {
                    task.DoneAt = null;
                    task.DoneUserId = Guid.Empty;
                }

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = type,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Member

    public async Task<OperationResult<bool>> AddMember(Guid userId, Guid taskId, TaskMemberViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var alreadyExists = await unit.WorkPackageTaskMember.AnyAsync(i
                    => i.TaskId == taskId && i.RecordId == model.RecordId);

                if (alreadyExists) return OperationResult<bool>.Success();

                var groupAccess = !model.IsGroup || await unit.GroupMembers
                    .AnyAsync(i => i.GroupId == model.RecordId && i.UserId == userId);
                if (!groupAccess) return OperationResult<bool>.Rejected();

                var access = await unit.WorkPackageMembers
                    .AnyAsync(i => i.RecordId == model.RecordId && i.IsGroup == model.IsGroup);
                if (!access) return OperationResult<bool>.Rejected();

                var member = new WorkPackageTaskMember
                {
                    PackageId = task.PackageId,
                    TaskId = taskId,
                    RecordId = model.RecordId,
                    IsGroup = model.IsGroup
                };
                await unit.AddAsync(member);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskMemberAdd,
                    UserId = userId,
                    WorkPackageTaskMember = member.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> RemoveMember(Guid userId, Guid taskId, Guid recordId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var taskMember = await unit.WorkPackageTaskMember
                    .SingleOrDefaultAsync(i => i.TaskId == taskId && i.RecordId == recordId);
                if (taskMember == null) return OperationResult<bool>.Rejected();

                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskMember.TaskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                unit.WorkPackageTaskMember.Remove(taskMember);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskMemberRemove,
                    UserId = userId,
                    WorkPackageTaskMember = taskMember.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Labels

    public async Task<OperationResult<bool>> AddLabel(Guid userId, Guid taskId, Guid labelId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var packageLabel = await unit.WorkPackageLabels.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == labelId);

                var alreadyExists = await unit.WorkPackageTaskLabels
                    .AnyAsync(i => i.TaskId == taskId && i.LabelId == labelId);

                if (alreadyExists) return OperationResult<bool>.Success();
                var label = new WorkPackageTaskLabel
                {
                    LabelId = labelId,
                    PackageId = task.PackageId,
                    TaskId = taskId
                };
                unit.WorkPackageTaskLabels.Add(label);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskLabelAdd,
                    UserId = userId,
                    PackageTaskLabel = label.ToViewModel(),
                    PackageTask = task.ToViewModel(),
                    PackageLabel = packageLabel.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> RemoveLabel(Guid userId, Guid taskId, Guid labelId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var taskLabel = await unit.WorkPackageTaskLabels
                    .SingleOrDefaultAsync(i => i.LabelId == labelId && i.TaskId == taskId);
                if (taskLabel == null) return OperationResult<bool>.Rejected();

                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskLabel.TaskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                var packageLabel = await unit.WorkPackageLabels.AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == labelId);
                unit.WorkPackageTaskLabels.Remove(taskLabel);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskLabelRemove,
                    UserId = userId,
                    PackageTaskLabel = taskLabel.ToViewModel(),
                    PackageTask = task.ToViewModel(),
                    PackageLabel = packageLabel.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Attachment

    public async Task<OperationResult<BulkDownloadResultViewModel>> BulkDownload(Guid userId, Guid id, Guid[] picked)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .Where(i => i.Id == id)
                    .Select(i => new { i.Title, i.PackageId })
                    .SingleOrDefaultAsync();

                if (task == null) return OperationResult<BulkDownloadResultViewModel>.NotFound();

                var access = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (access.Status != OperationResultStatus.Success || !access.Data)
                    return OperationResult<BulkDownloadResultViewModel>.Rejected();

                var paths = (await (
                        from attach in unit.WorkPackageTaskAttachments
                        join tsk in unit.WorkPackageTasks on attach.TaskId equals tsk.Id
                        where picked.Contains(tsk.Id)
                        select new { tsk.Title, attach.Path }
                    ).ToArrayAsync())
                    .GroupBy(i => i.Title)
                    .ToDictionary(k => k.Key, v => v.Select(o => o.Path).ToArray());

                var op = await _serviceProvider
                    .GetService<IStorageService>()
                    .BulkDownload(paths);


                if (op.Status != OperationResultStatus.Success)
                    return OperationResult<BulkDownloadResultViewModel>.Failed();

                op.Data.Seek(0, SeekOrigin.Begin);

                return OperationResult<BulkDownloadResultViewModel>
                    .Success(new BulkDownloadResultViewModel { Title = task.Title, Zip = op.Data });
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<BulkDownloadResultViewModel>.Failed();
        }
    }

    public async Task<OperationResult<bool>> EditAdvancedComment(Guid userId, Guid commentId, TitleViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var comment = await unit.AdvancedPlayerComments
                    .SingleOrDefaultAsync(c => c.Id == commentId);
                if (comment == null) return OperationResult<bool>.NotFound();
                if (userId != comment.UserId)
                {
                    var found = await (
                        from attachment in unit.WorkPackageTaskAttachments
                        join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                        where attachment.Id == comment.AttachmentId
                        select new { Attachment = attachment, Task = task }
                    ).AsNoTracking().SingleOrDefaultAsync();

                    if (found == null) return OperationResult<bool>.NotFound();
                    var checkAccess = await IsWorkPackageAdmin(unit, userId, found.Task.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();
                }

                comment.UpdatedAt = DateTime.UtcNow;
                comment.Message = model.Title;
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public Task<OperationResult<PdfAdvancedCommentViewModel>> PdfAdvanced(Guid userId, Guid attachmentId)
    {
        return Task.FromResult(OperationResult<PdfAdvancedCommentViewModel>.Failed());
        // try
        // {
        //     using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
        //     {
        //         var found = await (
        //             from attachment in unit.WorkPackageTaskAttachments
        //             join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
        //             where attachment.Id == attachmentId
        //             select new {Attachment = attachment, Task = task}
        //         ).AsNoTracking().SingleOrDefaultAsync();
        //
        //         if (found == null) return OperationResult<PdfAdvancedCommentViewModel>.NotFound();
        //         // if (userId != comment.UserId)
        //         // {
        //         //     var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
        //         //     if (checkAccess.Status != OperationResultStatus.Success)
        //         //         return OperationResult<PdfAdvancedCommentViewModel>.Rejected();
        //         // }
        //
        //         var comments = await unit.AdvancedPlayerComments.Where(c => c.AttachmentId == attachmentId)
        //             .AsNoTracking()
        //             .OrderBy(i => i.StartFrame)
        //             .ToArrayAsync();
        //
        //         var serverInfo = _serviceProvider.GetService<IServerInfo>();
        //         var uploadProvider = _serviceProvider.GetService<IUploadProvider>();
        //         var lastEdit = comments.Select(c => c.UpdatedAt ?? c.CreatedAt).Max();
        //         var downloadName = $"{found.Task.Title}_{lastEdit.GetTime()}";
        //         var pdfPath = Path.Combine(serverInfo.FilesRootPath, "pdf/advanced", $"{found.Task.Id}/{attachmentId}/{lastEdit.GetTime()}/{downloadName}.pdf");
        //         var pdfDir = Path.GetDirectoryName(pdfPath);
        //         if (!Directory.Exists(pdfDir)) Directory.CreateDirectory(pdfDir);
        //         if (!File.Exists(pdfPath))
        //         {
        //             IConversion conversion;
        //             Dictionary<string, string> result = new Dictionary<string, string>();
        //             var source = $"{serverInfo.FilesRootPath}{uploadProvider.RemoveUrlPrefix(found.Attachment.Path)}";
        //             foreach (var comment in comments)
        //             {
        //                 var destination = Path.Combine(pdfDir, comment.Id + ".png");
        //                 if (!File.Exists(destination))
        //                 {
        //                     conversion = await FFmpeg.Conversions.FromSnippet
        //                         .Snapshot(
        //                             source, 
        //                             destination, 
        //                             TimeSpan.FromSeconds(comment.StartFrame)
        //                         );
        //                     await conversion.Start();
        //                 }
        //                 
        //                 result.Add(destination, comment.Message);
        //             }
        //
        //             var htmlSource = Path.Combine(serverInfo.ReportsRootPath, "advanced-pdf.html");
        //             var html = File.ReadAllText(htmlSource).Replace("{{title}}", found.Attachment.Title);
        //             var content = new StringBuilder("");
        //
        //             foreach (var item in result)
        //             {
        //                 content.Append("<div class='page'>");
        //                 content.Append($"<img src='file://{item.Key}' alt='img' />");
        //                 content.Append($"<p>{item.Value}</p>");
        //                 content.Append($"</div>");
        //             }
        //             
        //             var markup = html.Replace("{{content}}", content.ToString());
        //             await _serviceProvider.GetService<IPdfBiz>().FromHtml(markup, pdfPath);
        //         }
        //         return OperationResult<PdfAdvancedCommentViewModel>.Success(new PdfAdvancedCommentViewModel
        //         {
        //             Stream = File.OpenRead(pdfPath),
        //             FileName = downloadName
        //         });
        //     }
        // }
        // catch (Exception ex)
        // {
        //     await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
        //     return OperationResult<PdfAdvancedCommentViewModel>.Failed();
        // }
    }

    public async Task<OperationResult<bool>> RemoveAdvancedComment(Guid userId, Guid commentId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var comment = await unit.AdvancedPlayerComments
                    .SingleOrDefaultAsync(c => c.Id == commentId);
                if (comment == null) return OperationResult<bool>.NotFound();

                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == comment.AttachmentId
                    select new { Attachment = attachment, Task = task }
                ).AsNoTracking().SingleOrDefaultAsync();

                if (found == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageAdmin(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();

                unit.AdvancedPlayerComments.Remove(comment);
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<AdvancedPlayerCommentViewModel>> CommentAdvanced(Guid userId,
        Guid attachmentId, EditAdvancedCommentViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == attachmentId
                    select new { Attachment = attachment, Task = task }
                ).AsNoTracking().SingleOrDefaultAsync();

                if (found == null) return OperationResult<AdvancedPlayerCommentViewModel>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<AdvancedPlayerCommentViewModel>.Rejected();

                var comment = new AdvancedPlayerComment
                {
                    Message = model.Message.Trim(),
                    AttachmentId = attachmentId,
                    StartFrame = model.StartFrame,
                    EndFrame = model.EndFrame,
                    UserId = userId
                };

                await unit.AdvancedPlayerComments.AddAsync(comment);
                await unit.SaveChangesAsync();

                return OperationResult<AdvancedPlayerCommentViewModel>.Success(comment.ToViewModel());
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<AdvancedPlayerCommentViewModel>.Failed();
        }
    }

    public async Task<OperationResult<AdvancedPlayerViewModel>> FetchAdvanced(Guid userId, Guid attachmentId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == attachmentId
                    select new { Attachment = attachment, Task = task }
                ).AsNoTracking().SingleOrDefaultAsync();

                if (found == null) return OperationResult<AdvancedPlayerViewModel>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<AdvancedPlayerViewModel>.Rejected();

                var comments = await unit.AdvancedPlayerComments
                    .Where(a => a.AttachmentId == attachmentId)
                    .OrderBy(o => o.StartFrame)
                    .AsNoTracking()
                    .ToArrayAsync();

                var shapes = await unit.AdvancedPlayerShapes
                    .Where(a => a.AttachmentId == attachmentId)
                    .OrderBy(o => o.StartFrame)
                    .AsNoTracking()
                    .ToArrayAsync();

                return OperationResult<AdvancedPlayerViewModel>.Success(new AdvancedPlayerViewModel
                {
                    Comments = comments.Select(i => i.ToViewModel()).ToArray(),
                    Shapes = shapes.Select(i => i.ToViewModel()).ToArray()
                });
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<AdvancedPlayerViewModel>.Failed();
        }
    }

    public async Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid taskId,
        StorageItemDto file)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks.AsNoTracking().SingleOrDefaultAsync(i => i.Id == taskId);
                if (task == null) return OperationResult<UploadResultViewModel>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<UploadResultViewModel>.Rejected();

                var projectPlanId = await unit.Projects
                    .Where(i => i.Id == task.ProjectId)
                    .Select(i => i.PlanInfoId)
                    .SingleOrDefaultAsync();

                var plan = await unit.UserPlanInfo.Where(i => i.Id == projectPlanId).SingleOrDefaultAsync();
                if (plan == null) return OperationResult<UploadResultViewModel>.Rejected();

                if (plan.AttachmentSize < file.FileSize)
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        AttachmentSize = true
                    });

                if (plan.UsedSpace + file.FileSize > plan.Space)
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        StorageSize = true
                    });

                plan.UsedSpace += file.FileSize;

                file.Section = UploadSection.WorkPackage;
                file.PlanId = plan.Id;
                file.RecordId = taskId;
                file.UserId = userId;

                var result = await _serviceProvider.GetService<IStorageService>().Upload(file);
                if (result.Status != OperationResultStatus.Success)
                    return OperationResult<UploadResultViewModel>.Rejected();
                var upload = new Upload
                {
                    Directory = result.Data.Directory,
                    Extension = result.Data.Extension,
                    Name = result.Data.FileName,
                    Path = result.Data.Url,
                    Public = false,
                    Section = UploadSection.WorkPackage,
                    Size = result.Data.FileSize,
                    RecordId = result.Data.RecordId,
                    ThumbnailPath = result.Data.ThumbnailUrl,
                    UserId = result.Data.UserId,
                    Type = result.Data.Type,
                    Id = result.Data.Id
                };
                var taskAttachment = new WorkPackageTaskAttachment
                {
                    Description = null,
                    Path = upload.Path,
                    Title = upload.Name,
                    ThumbnailPath = upload.ThumbnailPath,
                    Type = WorkPackageTaskAttachmentType.Upload,
                    PackageId = task.PackageId,
                    ProjectId = task.ProjectId,
                    TaskId = taskId,
                    UploadId = upload.Id,
                    UserId = userId,
                    SubProjectId = task.SubProjectId
                };
                await unit.WorkPackageTaskAttachments.AddAsync(taskAttachment);
                await unit.Uploads.AddAsync(upload);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskAttachmentAdd,
                    UserId = userId,
                    PackageTaskAttachment = taskAttachment.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<UploadResultViewModel>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<UploadResultViewModel>.Failed();
        }
    }

    public async Task<OperationResult<UploadResultViewModel>> BulkAttachment(Guid userId, Guid taskId,
        StorageItemDto file)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension.ToLower() != ".zip") return OperationResult<UploadResultViewModel>.Rejected();

                var task = await unit.WorkPackageTasks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);
                if (task == null) return OperationResult<UploadResultViewModel>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<UploadResultViewModel>.Rejected();

                var projectPlanId = await unit.Projects
                    .Where(i => i.Id == task.ProjectId)
                    .Select(i => i.PlanInfoId)
                    .SingleOrDefaultAsync();

                var plan = await unit.UserPlanInfo.Where(i => i.Id == projectPlanId).SingleOrDefaultAsync();
                if (plan == null) return OperationResult<UploadResultViewModel>.Rejected();

                if (plan.UsedSpace + file.FileSize > plan.Space)
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        StorageSize = true
                    });

                var subs = await unit.WorkPackageTasks
                    .Where(i => i.ParentId == taskId && !i.ArchivedAt.HasValue)
                    .AsNoTracking()
                    .Select(t => new SelectableItem<Guid>
                    {
                        Text = t.Title,
                        Value = t.Id
                    })
                    .ToArrayAsync();

                file.Section = UploadSection.WorkPackage;
                file.PlanId = plan.Id;
                file.RecordId = taskId;
                file.UserId = userId;

                var result = await _serviceProvider.GetService<IStorageService>().BulkUpload(file /*, subs*/);
                if (result.Status != OperationResultStatus.Success || !result.Data.Any())
                    return OperationResult<UploadResultViewModel>.Rejected();

                var uploads = result.Data.Select(u => new Upload
                {
                    Directory = u.Directory,
                    Extension = u.Extension,
                    Name = u.Name,
                    Path = u.Path,
                    Public = false,
                    Section = UploadSection.WorkPackage,
                    Size = u.Size,
                    RecordId = u.RecordId,
                    ThumbnailPath = u.ThumbnailPath,
                    UserId = u.UserId,
                    Type = u.Type,
                    Id = u.Id
                }).ToArray();

                var attachments = uploads.Select(u => new WorkPackageTaskAttachment
                {
                    Description = null,
                    Path = u.Path,
                    Title = u.Name,
                    ThumbnailPath = u.ThumbnailPath,
                    Type = WorkPackageTaskAttachmentType.Upload,
                    PackageId = task.PackageId,
                    ProjectId = task.ProjectId,
                    TaskId = u.RecordId,
                    UploadId = Guid.NewGuid(),
                    UserId = userId,
                    SubProjectId = task.SubProjectId
                }).ToArray();

                plan.UsedSpace += uploads.Sum(s => s.Size);
                await unit.WorkPackageTaskAttachments.AddRangeAsync(attachments);
                await unit.Uploads.AddRangeAsync(uploads);
                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskAttachmentBulkAdd,
                    UserId = userId,
                    PackageTask = task.ToViewModel(),
                    Affected = uploads.Length
                });
                return OperationResult<UploadResultViewModel>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<UploadResultViewModel>.Failed();
        }
    }


    public async Task<OperationResult<bool>> RemoveAttachment(Guid userId, Guid attachmentId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == attachmentId
                    select new { Attachment = attachment, Task = task }
                ).SingleOrDefaultAsync();

                if (found == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();

                var projectPlanId = await unit.Projects
                    .Where(i => i.Id == found.Task.ProjectId)
                    .Select(i => i.PlanInfoId)
                    .SingleOrDefaultAsync();

                var plan = await unit.UserPlanInfo.Where(i => i.Id == projectPlanId).SingleOrDefaultAsync();
                if (plan == null) return OperationResult<bool>.Rejected();

                var uploadService = _serviceProvider.GetService<IStorageService>();
                var result = await uploadService.Delete(found.Attachment.Path);
                if (result.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();

                var upload = await unit.Uploads.SingleOrDefaultAsync(u =>
                    u.Id == found.Attachment.UploadId.Value);
                if (upload != null)
                {
                    plan.UsedSpace -= upload.Size;
                    unit.Uploads.Remove(upload);
                }

                unit.WorkPackageTaskAttachments.Remove(found.Attachment);

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskAttachmentRemove,
                    UserId = userId,
                    PackageTaskAttachment = found.Attachment.ToViewModel(),
                    PackageTask = found.Task.ToViewModel()
                });
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> RenameAttachment(Guid userId, Guid attachmentId, TitleViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == attachmentId
                    select new { Attachment = attachment, Task = task }
                ).SingleOrDefaultAsync();

                if (found == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();

                var uploadService = _serviceProvider.GetService<IStorageService>();
                var result = await uploadService.Rename(found.Attachment.Path, model.Title);
                if (result.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();

                found.Attachment.Path = result.Data;
                found.Attachment.Title = Path.GetFileName(result.Data);

                var upload = await unit.Uploads.SingleOrDefaultAsync(u =>
                    u.Id == found.Attachment.UploadId.Value);

                if (upload != null)
                {
                    upload.Path = result.Data;
                    upload.Name = model.Title;
                }

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskAttachmentRename,
                    UserId = userId,
                    PackageTaskAttachment = found.Attachment.ToViewModel(),
                    PackageTask = found.Task.ToViewModel()
                });
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> CoverAttachment(Guid userId, Guid attachmentId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var found = await (
                    from attachment in unit.WorkPackageTaskAttachments
                    join task in unit.WorkPackageTasks on attachment.TaskId equals task.Id
                    where attachment.Id == attachmentId
                    select new { Attachment = attachment, Task = task }
                ).SingleOrDefaultAsync();

                if (found == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Rejected();


                if (!found.Task.CoverId.HasValue)
                {
                    found.Task.CoverId = found.Attachment.Id;
                    found.Attachment.IsCover = true;
                }
                else
                {
                    if (found.Task.CoverId == attachmentId)
                    {
                        found.Task.CoverId = null;
                        found.Attachment.IsCover = false;
                    }
                    else
                    {
                        var prev = await unit.WorkPackageTaskAttachments.SingleOrDefaultAsync(
                            a => a.Id == found.Task.CoverId);
                        if (prev != null) prev.IsCover = false;
                        found.Task.CoverId = attachmentId;
                    }
                }

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskAttachmentCover,
                    UserId = userId,
                    PackageTaskAttachment = found.Attachment.ToViewModel()
                });
                return OperationResult<bool>.Success();
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Vote

    public async Task<OperationResult<bool>> Vote(Guid userId, Guid taskId, VoteViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageEditor(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                if (task.VotePaused) return OperationResult<bool>.Rejected();

                var vote = await unit.WorkPackageTaskVotes.SingleOrDefaultAsync(i =>
                    i.UserId == userId && i.TaskId == taskId);
                if (vote == null)
                {
                    vote = new WorkPackageTaskVote
                    {
                        Vote = model.Vote,
                        PackageId = task.PackageId,
                        ProjectId = task.ProjectId,
                        TaskId = taskId,
                        UserId = userId,
                        SubProjectId = task.SubProjectId
                    };
                    await unit.WorkPackageTaskVotes.AddAsync(vote);
                }
                else
                {
                    vote.Vote = model.Vote;
                    vote.UpdatedAt = DateTime.UtcNow;
                }

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskVote,
                    UserId = userId,
                    PackageTaskVote = vote.ToViewModel(),
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> VoteSetting(Guid userId, Guid taskId, VoteSettingViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageAdmin(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                if (model.Paused.HasValue) task.VotePaused = model.Paused.Value;
                if (model.Private.HasValue) task.VotePrivate = model.Private.Value;
                if (model.Necessity.HasValue) task.VoteNecessity = model.Necessity.Value;

                await unit.SaveChangesAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskEdit,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> VoteClear(Guid userId, Guid taskId)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var task = await unit.WorkPackageTasks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(i => i.Id == taskId);

                if (task == null) return OperationResult<bool>.NotFound();

                var checkAccess = await IsWorkPackageAdmin(unit, userId, task.PackageId);
                if (checkAccess.Status != OperationResultStatus.Success) return checkAccess;

                await unit.WorkPackageTaskVotes.Where(i => i.TaskId == taskId).DeleteAsync();
                await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                {
                    Type = ActivityType.WorkPackageTaskVoteReset,
                    UserId = userId,
                    PackageTask = task.ToViewModel()
                });
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    #endregion

    #region Private

    private async Task<OperationResult<bool>> IsWorkPackageAdmin(
        ProjectManagementDbContext unit, Guid userId, Guid packageId)
    {
        var user = await unit.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == userId);
        if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
            return OperationResult<bool>.Rejected();

        var groupIds = await (
            from member in unit.GroupMembers
            join grp in unit.Groups on member.GroupId equals grp.Id
            where member.UserId == userId &&
                  !grp.ArchivedAt.HasValue &&
                  !member.DeletedAt.HasValue
            select grp.Id
        ).ToArrayAsync();

        var access = await (
                from member in unit.WorkPackageMembers
                join proj in unit.WorkPackages on member.PackageId equals proj.Id into tmp
                from proj in tmp.DefaultIfEmpty()
                where ((!member.IsGroup && member.RecordId == userId) ||
                       (member.IsGroup && groupIds.Contains(member.RecordId) &&
                        !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                        !proj.ArchivedAt.HasValue)) && member.PackageId == packageId &&
                      (member.Access == AccessType.Admin || member.Access == AccessType.Owner)
                select proj
            )
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .AnyAsync();

        if (!access) return OperationResult<bool>.Rejected();
        return OperationResult<bool>.Success(true);
    }

    private async Task<OperationResult<bool>> IsWorkPackageEditor(ProjectManagementDbContext unit, Guid userId,
        Guid packageId)
    {
        var user = await unit.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == userId);
        if (user == null || user.IsLocked || user.Blocked || user.DeletedAt.HasValue)
            return OperationResult<bool>.Rejected();

        var groupIds = await (
            from member in unit.GroupMembers
            join grp in unit.Groups on member.GroupId equals grp.Id
            where member.UserId == userId &&
                  !grp.ArchivedAt.HasValue &&
                  !member.DeletedAt.HasValue
            select grp.Id
        ).ToArrayAsync();

        var access = await (
                from member in unit.WorkPackageMembers
                join proj in unit.WorkPackages on member.PackageId equals proj.Id into tmp
                from proj in tmp.DefaultIfEmpty()
                where ((!member.IsGroup && member.RecordId == userId) ||
                       (member.IsGroup && groupIds.Contains(member.RecordId) &&
                        !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                        !proj.ArchivedAt.HasValue)) && member.PackageId == packageId &&
                      member.Access != AccessType.Visitor
                select proj
            )
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .AnyAsync();

        if (!access) return OperationResult<bool>.Rejected();
        return OperationResult<bool>.Success(true);
    }

    #endregion
}