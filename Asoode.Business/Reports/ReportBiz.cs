using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Reports;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Reports;
using Asoode.Data.Contexts;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Reports;

internal class ReportBiz : IReportBiz
{
    private readonly IServiceProvider _serviceProvider;

    public ReportBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<DashBoardViewModel>> Dashboard(Guid userId, DashboardDurationViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.IsLocked || user.Blocked)
                    return OperationResult<DashBoardViewModel>.Rejected();

                var groupIds = await (
                    from member in unit.GroupMembers
                    join grp in unit.Groups on member.GroupId equals grp.Id
                    where member.UserId == userId && !grp.ArchivedAt.HasValue
                    select grp.Id
                ).ToArrayAsync();

                var projectIds = await (
                    from member in unit.ProjectMembers
                    join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                    from proj in tmp.DefaultIfEmpty()
                    where (!member.IsGroup && member.RecordId == userId) ||
                          (member.IsGroup && groupIds.Contains(member.RecordId) &&
                           !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                           !proj.ArchivedAt.HasValue)
                    select proj.Id
                ).ToArrayAsync();

                var stats = await (
                    from task in unit.WorkPackageTasks
                    join pkg in unit.WorkPackages on task.PackageId equals pkg.Id
                    where
                        projectIds.Contains(pkg.ProjectId) &&
                        !pkg.DeletedAt.HasValue &&
                        !task.DeletedAt.HasValue
                    orderby task.CreatedAt
                    group task by task.State
                    into categoried
                    select new { State = categoried.Key, Total = categoried.Count() }
                ).AsNoTracking().ToArrayAsync();

                var report = await (
                    from task in unit.WorkPackageTasks
                    join activity in unit.Activities on task.Id equals activity.RecordId
                    where projectIds.Contains(task.ProjectId) &&
                          activity.CreatedAt >= model.Begin && activity.CreatedAt <= model.End
                    select activity
                ).AsNoTracking().ToArrayAsync();

                var totalTasks = stats.Select(i => i.Total).Sum();

                var events = await (
                    from member in unit.WorkPackageTaskMember
                    join task in unit.WorkPackageTasks on member.TaskId equals task.Id
                    where
                        (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                        !task.ArchivedAt.HasValue && !task.DeletedAt.HasValue &&
                        !(
                            task.State == WorkPackageTaskState.Canceled ||
                            task.State == WorkPackageTaskState.Done ||
                            task.State == WorkPackageTaskState.Duplicate
                        ) && (
                            (task.DueAt.HasValue && task.DueAt >= model.MonthBegin && task.DueAt <= model.MonthEnd) ||
                            (task.BeginAt.HasValue && task.BeginAt >= model.MonthBegin &&
                             task.DueAt <= model.MonthEnd) ||
                            (task.EndAt.HasValue && task.EndAt >= model.MonthBegin && task.EndAt <= model.MonthEnd)
                        )
                    orderby task.DueAt
                    select new { task.Id, task.Title, task.BeginAt, task.EndAt, task.DueAt, task.State }
                ).AsNoTracking().ToArrayAsync();

                var result = new DashBoardViewModel
                {
                    Events = events.Select(e => new DashBoardEventViewModel
                    {
                        Date = e.DueAt ?? e.EndAt ?? e.BeginAt ?? DateTime.MinValue,
                        Title = e.Title,
                        RecordId = e.Id,
                        State = e.State
                    }).ToArray(),
                    Progress = report
                        .GroupBy(i => i.CreatedAt.Date)
                        .Select(i =>
                        {
                            var blocked = i.Count(y => y.Type == ActivityType.WorkPackageTaskBlocked);
                            var blocker = i.Count(y => y.Type == ActivityType.WorkPackageTaskBlocker);
                            var paused = i.Count(y => y.Type == ActivityType.WorkPackageTaskPaused);
                            var unBlocked = i.Count(y => y.Type == ActivityType.WorkPackageTaskUnBlock);
                            var done = i.Count(y => y.Type == ActivityType.WorkPackageTaskDone);
                            var created = i.Count(y => y.Type == ActivityType.WorkPackageTaskAdd);
                            return new DashBoardProgressViewModel
                            {
                                Date = i.Key,
                                Blocked = blocked + blocker + paused,
                                Done = done + unBlocked,
                                Total = created
                            };
                        }).ToArray(),
                    Overall = new DashBoardOverallViewModel
                    {
                        Blocked = stats.SingleOrDefault(s => s.State == WorkPackageTaskState.Blocked)?.Total ?? 0,
                        Done = stats.SingleOrDefault(s => s.State == WorkPackageTaskState.Done)?.Total ?? 0,
                        InProgress = stats.SingleOrDefault(s => s.State == WorkPackageTaskState.InProgress)?.Total ?? 0,
                        Total = totalTasks
                    }
                };
                return OperationResult<DashBoardViewModel>.Success(result);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<DashBoardViewModel>.Failed();
        }
    }

    public async Task<OperationResult<WorkPackageTaskViewModel[]>> Activities(Guid userId, Guid? id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                if (user == null || user.IsLocked || user.Blocked)
                    return OperationResult<WorkPackageTaskViewModel[]>.Rejected();

                WorkPackageTaskViewModel[] logs;
                if (!id.HasValue)
                {
                    var groupIds = await (
                        from member in unit.GroupMembers
                        join grp in unit.Groups on member.GroupId equals grp.Id
                        where member.UserId == userId
                        select grp.Id
                    ).ToArrayAsync();

                    var projectIds = await (
                        from member in unit.ProjectMembers
                        join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                        from proj in tmp.DefaultIfEmpty()
                        where (member.RecordId == userId || groupIds.Contains(member.RecordId)) &&
                              !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                              !proj.ArchivedAt.HasValue
                        select proj.Id
                    ).ToArrayAsync();

                    var tasks = await (
                        from task in unit.WorkPackageTasks
                        join activity in unit.WorkPackageTaskTimes on task.Id equals activity.TaskId
                        where activity.UserId == userId && projectIds.Contains(task.ProjectId)
                        orderby activity.CreatedAt descending
                        select task
                    ).Distinct().Take(5).AsNoTracking().ToArrayAsync();
                    logs = tasks.Select(t => t.ToViewModel()).ToArray();
                }
                else
                {
                    var groupMembers = await unit.GroupMembers
                        .Where(i => i.GroupId == id.Value)
                        .Select(i => i.UserId)
                        .ToArrayAsync();

                    if (!groupMembers.Contains(userId)) return OperationResult<WorkPackageTaskViewModel[]>.Rejected();

                    var projectIds = await (
                        from member in unit.ProjectMembers
                        join proj in unit.Projects on member.ProjectId equals proj.Id into tmp
                        from proj in tmp.DefaultIfEmpty()
                        where member.RecordId == id.Value &&
                              !member.DeletedAt.HasValue && !proj.DeletedAt.HasValue &&
                              !proj.ArchivedAt.HasValue
                        select proj.Id
                    ).ToArrayAsync();

                    var tasks = await (
                        from task in unit.WorkPackageTasks
                        join activity in unit.WorkPackageTaskTimes on task.Id equals activity.TaskId
                        where groupMembers.Contains(activity.UserId) && projectIds.Contains(task.ProjectId)
                        orderby activity.CreatedAt descending
                        select task
                    ).Distinct().Take(5).AsNoTracking().ToArrayAsync();
                    logs = tasks.Select(t => t.ToViewModel()).ToArray();
                }

                var taskIds = logs.Select(l => l.Id).ToArray();
                var workPackageTaskMembers = await unit.WorkPackageTaskMember
                    .Where(l => taskIds.Contains(l.TaskId))
                    .ToArrayAsync();

                var workPackageTaskLabels = await unit.WorkPackageTaskLabels
                    .Where(l => taskIds.Contains(l.TaskId) && !l.DeletedAt.HasValue)
                    .OrderByDescending(i => i.CreatedAt)
                    .AsNoTracking()
                    .ToArrayAsync();

                var attachmentsCount = await unit.WorkPackageTaskAttachments
                    .Where(i => taskIds.Contains(i.TaskId))
                    .GroupBy(t => t.TaskId)
                    .Select(r => new { TaskId = r.Key, Count = r.Count() })
                    .ToListAsync();

                var interactionsCount = await unit.WorkPackageTaskInteractions
                    .Where(i => taskIds.Contains(i.TaskId) && i.UserId == userId)
                    .ToListAsync();

                var subTasksCount = await unit.WorkPackageTasks
                    .Where(i => taskIds.Contains(i.Id) && i.ParentId.HasValue)
                    .Select(r => new { r.ParentId, Done = r.DoneAt.HasValue })
                    .ToListAsync();

                var voteCount = await unit.WorkPackageTaskVotes
                    .Where(i => taskIds.Contains(i.Id))
                    .ToListAsync();

                foreach (var t in logs)
                {
                    t.Members = workPackageTaskMembers.Where(i => i.TaskId == t.Id)
                        .Select(i => i.ToViewModel()).ToArray();
                    t.Labels = workPackageTaskLabels.Where(i => i.TaskId == t.Id)
                        .Select(i => i.ToViewModel()).ToArray();
                    t.HasDescription = !string.IsNullOrEmpty(t.Description);
                    t.Description = "";

                    // TODO: add other details to task

                    t.AttachmentCount = attachmentsCount
                        .SingleOrDefault(a => a.TaskId == t.Id)?.Count ?? 0;

                    t.SubTasksCount = subTasksCount.Count(a => a.ParentId == t.Id);
                    t.SubTasksDone = subTasksCount.Count(a => a.ParentId == t.Id && a.Done);

                    t.DownVotes = voteCount.Count(a => a.TaskId == t.Id && !a.Vote);
                    t.UpVotes = voteCount.Count(a => a.TaskId == t.Id && a.Vote);

                    t.Watching = interactionsCount.Any(i => i.TaskId == t.Id && i.Watching == true);
                }

                return OperationResult<WorkPackageTaskViewModel[]>.Success(logs);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<WorkPackageTaskViewModel[]>.Failed();
        }
    }
}