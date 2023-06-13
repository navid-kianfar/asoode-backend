using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Logging;
using Asoode.Data.Contexts;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Background.Services;

public class TaskDuePastService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private bool _lock;
    private Timer _timer;

    public TaskDuePastService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lock = false;
        _timer = new Timer(
            async o => await DoWork(o),
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(1)
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async Task DoWork(object state)
    {
        if (_lock)
        {
            Console.WriteLine("TaskDuePastService : progress!!!!");
            return;
        }

        var host = _serviceProvider.GetService<IHost>();
        using (var scope = host.Services.CreateScope())
        {
            try
            {
                _lock = true;
                Console.WriteLine("TaskDuePastService : started");
                var now = DateTime.UtcNow;
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                using (var unit = scope.ServiceProvider.GetService<ProjectManagementDbContext>())
                {
                    var passed = await unit.WorkPackageTasks
                        .Where(t =>
                            t.State != WorkPackageTaskState.Canceled &&
                            t.State != WorkPackageTaskState.Done &&
                            t.State != WorkPackageTaskState.Duplicate
                            &&
                            (
                                (t.DueAt.HasValue && t.DueAt.Value < now && !t.LastDuePassedNotified.HasValue) ||
                                (t.EndAt.HasValue && t.EndAt.Value < now && !t.LastEndPassedNotified.HasValue)
                            )
                        ).ToArrayAsync();

                    if (passed.Any())
                    {
                        var result = new List<NotificationViewModel>();
                        var translateBiz = scope.ServiceProvider.GetService<ITranslateBiz>();
                        var taskIds = passed.Select(i => i.Id).ToArray();
                        var taskMembers = await unit.WorkPackageTaskMember
                            .Where(i => taskIds.Contains(i.TaskId))
                            .AsNoTracking()
                            .ToArrayAsync();
                        var packageIds = passed.Select(i => i.PackageId).ToArray();
                        var packages = await unit.WorkPackages
                            .Where(i => packageIds.Contains(i.Id))
                            .AsNoTracking()
                            .ToArrayAsync();
                        var packageMembers = await unit.WorkPackageMembers
                            .Where(i => packageIds.Contains(i.PackageId))
                            .AsNoTracking()
                            .ToArrayAsync();
                        var groupIds = packageMembers
                            .Where(i => i.IsGroup)
                            .Select(i => i.RecordId)
                            .ToArray();
                        var groupMembers = await unit.GroupMembers
                            .Where(i => groupIds.Contains(i.GroupId))
                            .AsNoTracking()
                            .ToArrayAsync();

                        var grouped = passed
                            .GroupBy(i => i.PackageId)
                            .Select(i => new
                            {
                                PackageId = i.Key,
                                Tasks = i.ToArray()
                            })
                            .ToArray();
                        var allUserIds = packageMembers
                            .Where(i => !i.IsGroup)
                            .Select(i => i.RecordId)
                            .ToArray()
                            .Concat(
                                groupMembers.Select(i => i.UserId).ToArray()
                            ).Distinct().ToArray();
                        var allUsers = await unit.WebPushes
                            .Where(i => allUserIds.Contains(i.UserId) && i.Enabled)
                            .AsNoTracking()
                            .ToArrayAsync();
                        foreach (var grp in grouped)
                        {
                            var package = packages.Single(i => i.Id == grp.PackageId);
                            var filteredMembers = packageMembers
                                .Where(p => p.PackageId == grp.PackageId)
                                .ToArray();
                            var individuals = filteredMembers
                                .Where(i => !i.IsGroup)
                                .Select(i => i.RecordId)
                                .ToArray();
                            var teams = filteredMembers
                                .Where(i => i.IsGroup)
                                .Select(i => i.RecordId)
                                .ToArray();
                            var teamMembers = groupMembers
                                .Where(m => teams.Contains(m.GroupId))
                                .Select(i => i.UserId)
                                .ToArray();
                            var members = individuals.Concat(teamMembers).Distinct().ToArray();

                            foreach (var task in grp.Tasks)
                            {
                                Guid[] currentMemberIds;
                                if (package.TaskVisibility == WorkPackageTaskVisibility.MembersOnly)
                                {
                                    var currentMembers = taskMembers
                                        .Where(i => i.TaskId == task.Id)
                                        .ToArray();
                                    var currentIndividuals = currentMembers
                                        .Where(i => !i.IsGroup)
                                        .Select(i => i.RecordId)
                                        .ToArray();
                                    var currentTeams = currentMembers
                                        .Where(i => i.IsGroup)
                                        .Select(i => i.RecordId)
                                        .ToArray();
                                    var currentTeamMembers = groupMembers
                                        .Where(m => currentTeams.Contains(m.GroupId))
                                        .Select(i => i.UserId)
                                        .ToArray();
                                    currentMemberIds = currentIndividuals.Concat(currentTeamMembers).Distinct()
                                        .ToArray();
                                }
                                else
                                {
                                    currentMemberIds = members;
                                }

                                if (task.DueAt.HasValue && task.DueAt.Value < now) task.LastDuePassedNotified = now;
                                if (task.EndAt.HasValue && task.EndAt.Value < now) task.LastEndPassedNotified = now;

                                var culture = configuration["Setting:I18n:Default"];

                                result.Add(new NotificationViewModel
                                {
                                    Title = translateBiz.Get("PUSH_TASK_DUE_PASSED_TITLE", culture),
                                    Description = $"{task.Title}\r\n{task.Description}",
                                    Data = null,
                                    Avatar = "",
                                    Type = ActivityType.None,
                                    Url =
                                        $"https://panel.{configuration["Setting:Domain"]}/work-package/{grp.PackageId}/?taskId={task.Id}",
                                    Users = currentMemberIds,
                                    UserId = task.UserId,
                                    PushUsers = allUsers
                                        .Where(u => currentMemberIds.Contains(u.UserId))
                                        .Select(x => x.ToViewModel())
                                        .ToArray()
                                });
                            }
                        }

                        scope.ServiceProvider.GetService<BulkQueue>().Notifications(result);
                        await unit.SaveChangesAsync();
                    }
                }

                Console.WriteLine("TaskDuePastService : finished");
            }
            catch (Exception ex)
            {
                await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
                Console.WriteLine("TaskDuePastService : failed!!!!");
            }
            finally
            {
                _lock = false;
            }
        }
    }
}