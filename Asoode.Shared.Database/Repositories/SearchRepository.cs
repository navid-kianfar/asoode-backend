using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class SearchRepository : ISearchRepository
{
    private readonly ILoggerService _loggerService;
    private readonly ReportsContext _context;

    public SearchRepository(ILoggerService loggerService, ReportsContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    public async Task<CombinedGroupMemberDto[]> GetAllGroups(Guid userId)
    {
        try
        {
            var allGroups = await (
                from grp in _context.Groups
                join member in _context.GroupMembers on grp.Id equals member.GroupId
                where member.UserId == userId && !grp.ArchivedAt.HasValue && !member.DeletedAt.HasValue
                select new { Group = grp, Member = member }
            ).AsNoTracking().ToArrayAsync();

            return allGroups.Select(g => new CombinedGroupMemberDto
            {
                Group = g.Group.ToDto(),
                Member = g.Member.ToDto()
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllGroups", e);
            return Array.Empty<CombinedGroupMemberDto>();
        }
    }

    public async Task<CombinedProjectMemberDto[]> GetAllProjects(Guid userId, Guid[] groupIds)
    {
        try
        {
            var allProjects = await (
                from proj in _context.Projects
                join member in _context.ProjectMembers on proj.Id equals member.ProjectId
                where
                    !proj.ArchivedAt.HasValue &&
                    !proj.DeletedAt.HasValue &&
                    (member.RecordId == userId || groupIds.Contains(member.RecordId))
                select new { Project = proj, Member = member }
            ).AsNoTracking().ToArrayAsync();

            return allProjects.Select(g => new CombinedProjectMemberDto
            {
                Project = g.Project.ToDto(),
                Member = g.Member.ToDto()
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllGroups", e);
            return Array.Empty<CombinedProjectMemberDto>();
        }
    }

    public async Task<CombinedWorkPackageMemberDto[]> GetAllWorkPackages(Guid userId, Guid[] allGroupIds, Guid[] allProjectIds)
    {
        try
        {
            var allPackages = await (
                from pkg in _context.WorkPackages
                join member in _context.WorkPackageMembers on pkg.Id equals member.PackageId
                where
                    !pkg.ArchivedAt.HasValue &&
                    !pkg.DeletedAt.HasValue &&
                    (member.RecordId == userId || allGroupIds.Contains(member.RecordId))
                select new { Package = pkg, Member = member }
            ).AsNoTracking().ToArrayAsync();

            return allPackages.Select(g => new CombinedWorkPackageMemberDto
            {
                Package = g.Package.ToDto(),
                Member = g.Member.ToDto()
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllWorkPackages", e);
            return Array.Empty<CombinedWorkPackageMemberDto>();
        }
    }

    public async Task<MemberInfoDto[]> GetAllUsers(Guid[] everyOne, string search)
    {
        try
        {
            var filteredMembers = await _context.Users
                .Where(u =>
                    everyOne.Contains(u.Id) && (
                        u.FirstName.Contains(search) ||
                        u.Bio.Contains(search) ||
                        u.Email.Contains(search) ||
                        u.Phone.Contains(search) ||
                        u.Username.Contains(search) ||
                        u.LastName.Contains(search)
                    )
                )
                .AsNoTracking()
                .ToArrayAsync();
            
            return filteredMembers.Select(i => i.ToMemberInfoDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllUsers", e);
            return Array.Empty<MemberInfoDto>();
        }
    }

    public async Task<CombinedTaskListNameDto[]> GetAllTasks(Guid[] allPackageIds, string search)
    {
        try
        {
            var allTasks = await (
                from task in _context.WorkPackageTasks
                join list in _context.WorkPackageLists on task.ListId equals list.Id
                where allPackageIds.Contains(task.PackageId) && (
                    task.Description.Contains(search) ||
                    task.Title.Contains(search)
                )
                select new { Task = task, List = list.Title }
            ).AsNoTracking().ToArrayAsync();

            return allTasks.Select(i => new CombinedTaskListNameDto
            {
                Task = i.Task.ToDto(),
                List = i.List
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllTasks", e);
            return Array.Empty<CombinedTaskListNameDto>();
        }
    }

    public async Task<UploadDto[]> GetAllUploads(Guid userId, string search)
    {
        try
        {
            var allFiles = await _context.Uploads.Where(i =>
                i.UserId == userId && i.Section == UploadSection.Storage && (
                    i.Directory.Contains(search) ||
                    i.Name.Contains(search) ||
                    i.Path.Contains(search)
                )
            ).AsNoTracking().ToArrayAsync();

            return allFiles.Select(i => i.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllUploads", e);
            return Array.Empty<UploadDto>();
        }
    }

    public async Task<CombinedTaskMemberUserDto[]> GetAllTaskMembers(Guid[] allTaskIds)
    {
        try
        {
            var allMembers = await (
                from user in _context.Users
                join member in _context.WorkPackageTaskMember on user.Id equals member.RecordId
                where !member.IsGroup && allTaskIds.Contains(member.TaskId)
                select new { User = user, TaskMember = member }
            ).AsNoTracking().ToArrayAsync();

            return allMembers.Select(i => new CombinedTaskMemberUserDto
            {
                Member = i.TaskMember.ToDto(),
                User = i.User.ToMemberInfoDto(),
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllTaskMembers", e);
            return Array.Empty<CombinedTaskMemberUserDto>();
        }
    }

    public async Task<CombinedLabelTaskLabelDto[]> GetAllLabeledTasks(Guid[] allTaskIds)
    {
        try
        {
            var allLabels = await (
                from label in _context.WorkPackageLabels
                join taskLabel in _context.WorkPackageTaskLabels on label.Id equals taskLabel.LabelId
                where allTaskIds.Contains(taskLabel.TaskId)
                select new { Label = label, TaskLabel = taskLabel }
            ).AsNoTracking().ToArrayAsync();

            return allLabels.Select(i => new CombinedLabelTaskLabelDto
            {
                Label = i.Label.ToDto(),
                TaskLabel = i.TaskLabel.ToDto(),
            }).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchRepository.GetAllLabeledTasks", e);
            return Array.Empty<CombinedLabelTaskLabelDto>();
        }
    }
}