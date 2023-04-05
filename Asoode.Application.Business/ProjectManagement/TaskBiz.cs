using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.ProjectManagement;
using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;
using Asoode.Application.Core.ViewModels.Storage;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Asoode.Application.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.ProjectManagement
{
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

                    int counter = 2;
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
                            PackageTask = task.ToViewModel(new[] {member.ToViewModel()}),
                        });

                        return OperationResult<bool>.Success(true);
                    }

                    counter = model.Count.Value;
                    List<WorkPackageTask> tasks = new List<WorkPackageTask>();
                    List<WorkPackageTaskMember> members = new List<WorkPackageTaskMember>();
                    for (int i = 0; i < model.Count.Value; i++)
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
                        .Where(l => (l.TaskId == taskId) && l.UserId == userId && !l.DeletedAt.HasValue)
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
                            (!task.ArchivedAt.HasValue && !task.DeletedAt.HasValue) &&
                            (
                                task.State != WorkPackageTaskState.Canceled &&
                                task.State != WorkPackageTaskState.Done &&
                                task.State != WorkPackageTaskState.Duplicate
                            ) &&
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
                        select new {Task = task, List = list.Title}
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
                        PackageTask = task.ToViewModel(),
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
                            UserId = userId,
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
                        PackageTask = task.ToViewModel(),
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
                        PackageTask = task.ToViewModel(),
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

                    int orders = 1;
                    tasks.ForEach((e) => { e.Order = orders++; });

                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageTaskReposition,
                        UserId = userId,
                        PackageTask = tasks.Find(i => i.Id == taskId).ToViewModel(),
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
                    int counter = 1;
                    source.ForEach(s => s.Order = counter++);
                    counter = 1;
                    destination.ForEach(s => s.Order = counter++);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageTaskMove,
                        UserId = userId,
                        PackageTask = pop.ToViewModel(),
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
                        PackageTask = task.ToViewModel(),
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
                        PackageTask = task.ToViewModel(),
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

                            if (oldState == WorkPackageTaskState.Blocker)
                            {
                                type = ActivityType.WorkPackageTaskUnBlock;
                                // TODO: handle blocker
                            }

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
                        PackageTask = task.ToViewModel(),
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
                        TaskId = taskId,
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

        public async Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid taskId,
            UploadedFileViewModel file)
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


                    var result = await _serviceProvider.GetService<IUploadProvider>().Upload(new StoreViewModel
                    {
                        File = file,
                        Section = UploadSection.WorkPackage,
                        RecordId = taskId,
                        UserId = userId
                    });
                    if (result.Status != OperationResultStatus.Success)
                        return OperationResult<UploadResultViewModel>.Rejected();
                    var upload = new Upload
                    {
                        Directory = result.Data.Directory,
                        Extension = result.Data.Extension,
                        Name = result.Data.Name,
                        Path = result.Data.Path,
                        Public = false,
                        Section = UploadSection.WorkPackage,
                        Size = result.Data.Size,
                        RecordId = result.Data.RecordId,
                        ThumbnailPath = result.Data.ThumbnailPath,
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
                        SubProjectId = task.SubProjectId,
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

        public async Task<OperationResult<UploadResultViewModel>> BulkAttachment(Guid userId, Guid taskId, UploadedFileViewModel file)
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

                    var subs = await unit.WorkPackageTasks
                        .Where(i => i.ParentId == taskId && !i.ArchivedAt.HasValue)
                        .AsNoTracking()
                        .Select(t => new SelectableItem<Guid>
                        {
                            Text = t.Title,
                            Value = t.Id
                        })
                        .ToArrayAsync();
                    
                    var result = await _serviceProvider.GetService<IUploadProvider>().BulkUpload(new StoreViewModel
                    {
                        File = file,
                        Section = UploadSection.WorkPackage,
                        RecordId = taskId,
                        UserId = userId,
                        Subs = subs
                    });
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
                        UploadId = IncrementalGuid.NewId(),
                        UserId = userId,
                        SubProjectId = task.SubProjectId,
                    }).ToArray();
                    
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
                        select new {Attachment = attachment, Task = task}
                    ).SingleOrDefaultAsync();

                    if (found == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var uploadService = _serviceProvider.GetService<IUploadProvider>();
                    var result = await uploadService.Delete(found.Attachment.Path, UploadSection.WorkPackage);
                    if (result.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var upload = await unit.Uploads.SingleOrDefaultAsync(u =>
                        u.Id == found.Attachment.UploadId.Value);
                    if (upload != null)
                    {
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
                        select new {Attachment = attachment, Task = task}
                    ).SingleOrDefaultAsync();

                    if (found == null) return OperationResult<bool>.NotFound();

                    var checkAccess = await IsWorkPackageEditor(unit, userId, found.Task.PackageId);
                    if (checkAccess.Status != OperationResultStatus.Success)
                        return OperationResult<bool>.Rejected();

                    var uploadService = _serviceProvider.GetService<IUploadProvider>();
                    var result = await uploadService.Rename(new RenameAttachmentViewModel
                    {
                        Name = model.Title,
                        Path = found.Attachment.Path,
                        Section = UploadSection.WorkPackage
                    });
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
                        select new {Attachment = attachment, Task = task}
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
                        PackageTaskAttachment = found.Attachment.ToViewModel(),
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
                        PackageTask = task.ToViewModel(),
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

                    // TODO: use linqtodb
                    // await unit.WorkPackageTaskVotes.Where(i => i.TaskId == taskId).DeleteAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.WorkPackageTaskVoteReset,
                        UserId = userId,
                        PackageTask = task.ToViewModel(),
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
                        PackageTask = task.ToViewModel(),
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
                           (member.IsGroup && groupIds.Contains(member.RecordId)) &&
                           !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                           !proj.ArchivedAt.HasValue) && member.PackageId == packageId &&
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
                           (member.IsGroup && groupIds.Contains(member.RecordId)) &&
                           !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                           !proj.ArchivedAt.HasValue) && member.PackageId == packageId &&
                          (member.Access != AccessType.Visitor)
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
}