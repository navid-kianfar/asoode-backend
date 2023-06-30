using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class TimeSpentRepository : ITimeSpentRepository
{
    private readonly ILoggerService _loggerService;
    private readonly ReportsContext _context;

    public TimeSpentRepository(ILoggerService loggerService, ReportsContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    public async Task<TimeSpentDto[]> UserTimeSpents(Guid userId, DateTime begin, DateTime end)
    {
        try
        {
            var data = await (
                from time in _context.WorkPackageTaskTimes
                join task in _context.WorkPackageTasks on time.TaskId equals task.Id
                join list in _context.WorkPackageLists on task.ListId equals list.Id
                where time.UserId == userId && time.Begin >= begin && time.End <= end
                orderby time.Begin
                select new { Task = task, Time = time, List = list.Title }
            ).AsNoTracking().ToArrayAsync();
            
            return data.Select(i =>
            {
                var task = i.Task.ToDto();
                task.ListName = i.List;
                return new TimeSpentDto
                {
                    Task = task,
                    Time = i.Time.ToDto()
                };
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TimeSpentRepository.UserTimeSpents", e);
            return Array.Empty<TimeSpentDto>();
        }
    }

    public async Task<UserDto?> GetUser(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == userId);
            return user?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TimeSpentRepository.GetUser", e);
            return null;
        }
    }

    public async Task<GroupMemberDto?> GroupAccess(Guid userId, Guid groupId)
    {
        try
        {
            var access = await _context.GroupMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.UserId == userId && i.GroupId == groupId);
            return access?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TimeSpentRepository.GroupAccess", e);
            return null;
        }
    }

    public async Task<Guid[]> UserPackages(Guid groupId)
    {
        try
        {
            return await _context.WorkPackageMembers
                .Where(i => i.RecordId == groupId)
                .Select(i => i.PackageId)
                .ToArrayAsync();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TimeSpentRepository.UserPackages", e);
            return Array.Empty<Guid>();
        }
    }

    public async Task<TimeSpentDto[]> GroupTimeSpent(Guid userId, Guid groupId, GroupMemberDto access, Guid[] packages, DateTime begin, DateTime end)
    {
        try
        {
            var data = await (
                from time in _context.WorkPackageTaskTimes
                join task in _context.WorkPackageTasks on time.TaskId equals task.Id
                join list in _context.WorkPackageLists on task.ListId equals list.Id
                where
                    packages.Contains(time.PackageId) &&
                    time.Begin >= begin &&
                    time.End <= end &&
                    (
                        access.Access == AccessType.Admin ||
                        access.Access == AccessType.Owner ||
                        time.UserId == userId
                    )
                orderby time.Begin
                select new { Task = task, Time = time, List = list.Title }
            ).AsNoTracking().ToArrayAsync();
            
            return data.Select(i =>
            {
                var task = i.Task.ToDto();
                task.ListName = i.List;
                return new TimeSpentDto
                {
                    Task = task,
                    Time = i.Time.ToDto()
                };
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TimeSpentRepository.UserPackages", e);
            return Array.Empty<TimeSpentDto>();
        }
    }
}