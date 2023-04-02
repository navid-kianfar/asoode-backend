using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Contracts.TimeSpent;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Reports;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.TimeSpent
{
    internal class TimeSpentBiz : ITimeSpentBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public TimeSpentBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task<OperationResult<TimeSpentViewModel[]>> TimeSpents(Guid userId, DurationViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked) return OperationResult<TimeSpentViewModel[]>.Rejected();
                    
                    var data = await (
                        from time in unit.WorkPackageTaskTimes
                        join task in unit.WorkPackageTasks on time.TaskId equals task.Id
                        join list in unit.WorkPackageLists on task.ListId equals list.Id
                        where time.UserId == userId && time.Begin >= model.Begin && time.End <= model.End
                        orderby time.Begin
                        select new { Task = task, Time = time, List = list.Title }
                    ).AsNoTracking().ToArrayAsync();

                    var result = data.Select(i =>
                    {
                        var task = i.Task.ToViewModel();
                        task.ListName = i.List;
                        return new TimeSpentViewModel
                        {
                            Task = task,
                            Time = i.Time.ToViewModel()
                        };
                    }).ToArray();
                    
                    return OperationResult<TimeSpentViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<TimeSpentViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<TimeSpentViewModel[]>> GroupTimeSpents(Guid userId, Guid groupId, DurationViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked) return OperationResult<TimeSpentViewModel[]>.Rejected();

                    var access = await unit.GroupMembers
                        .AsNoTracking()
                        .SingleOrDefaultAsync(i => i.UserId == userId && i.GroupId == groupId);
                    if (access == null) return OperationResult<TimeSpentViewModel[]>.Rejected();

                    var packages = await unit.WorkPackageMembers
                        .Where(i => i.RecordId == groupId)
                        .Select(i => i.PackageId)
                        .ToArrayAsync();

                    var data = await (
                        from time in unit.WorkPackageTaskTimes
                        join task in unit.WorkPackageTasks on time.TaskId equals task.Id
                        join list in unit.WorkPackageLists on task.ListId equals list.Id
                        where 
                            packages.Contains(time.PackageId) && 
                            time.Begin >= model.Begin && 
                            time.End <= model.End &&
                            (
                                access.Access == AccessType.Admin || 
                                access.Access == AccessType.Owner || 
                                time.UserId == userId
                            )
                        orderby time.Begin
                        select new { Task = task, Time = time, List = list.Title }
                    ).AsNoTracking().ToArrayAsync();

                    var result = data.Select(i =>
                    {
                        var task = i.Task.ToViewModel();
                        task.ListName = i.List;
                        return new TimeSpentViewModel
                        {
                            Task = task,
                            Time = i.Time.ToViewModel()
                        };
                    }).ToArray();
                    
                    return OperationResult<TimeSpentViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<TimeSpentViewModel[]>.Failed();
            }
        }
    }
}