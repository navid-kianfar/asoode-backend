using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class StorageRepository : IStorageRepository
{
    private const string IGNORE_FILE = "ignore.me.txt";
    private readonly ILoggerService _loggerService;
    private readonly StorageDbContext _context;

    public StorageRepository(ILoggerService loggerService, StorageDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<UploadDto[]> GetDirectories(string key, string directory)
    {
        try
        {
            var directories = await _context.Uploads.Where(u =>
                    u.Path.Contains(key) &&
                    u.Directory != directory
                )
                .AsNoTracking()
                .ToArrayAsync();

            return directories.Select(u => u.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.GetDirectories", e);
            return Array.Empty<UploadDto>();
        }
    }

    public async Task<UploadDto[]> GetFiles(string directory)
    {
        try
        {
            var files = await _context.Uploads.Where(u =>
                    u.Directory == directory &&
                    u.Name != IGNORE_FILE
                )
                .AsNoTracking()
                .ToArrayAsync();
            return files.Select(u => u.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.GetFiles", e);
            return Array.Empty<UploadDto>();
        }
    }

    public async Task<UserDto?> FindUser(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == userId);
            return user?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.FindUser", e);
            return null;
        }
    }

    public async Task<Guid[]> FindGroupIds(Guid userId)
    {
        return await _context.GroupMembers
            .Where(i => i.UserId == userId)
            .Select(i => i.GroupId)
            .ToArrayAsync();
    }

    public async Task<ProjectDto[]> FindProjects(Guid userId)
    {
        var groupIds = await FindGroupIds(userId);
        var projects = await (
            from proj in _context.Projects join
            mem in _context.ProjectMembers on proj.Id equals mem.ProjectId
            where mem.RecordId == userId || groupIds.Contains(mem.RecordId)
            select proj
        ).AsNoTracking().Distinct().ToArrayAsync();
        return projects.Select(p => p.ToDto()).ToArray();
    }

    private async Task<Guid[]> FindProjectIds(Guid userId, Guid[] groupIds)
    {
        return await (
            from proj in _context.Projects join
            mem in _context.ProjectMembers on proj.Id equals mem.ProjectId
            where mem.RecordId == userId || groupIds.Contains(mem.RecordId)
            select proj.Id
        ).Distinct().ToArrayAsync();
    }

    private async Task<Guid[]> FindWorkPackageIds(Guid userId, Guid[] projectIds, Guid[] groupIds)
    {
        return await (
            from pkg in _context.WorkPackages join
            mem in _context.WorkPackageMembers on pkg.Id equals mem.PackageId
            where mem.RecordId == userId || groupIds.Contains(mem.RecordId)
            select pkg.Id
        ).Distinct().ToArrayAsync();
    }

    public async Task<WorkPackageDto[]> FindWorkPackages(Guid userId, Guid projectId)
    {
        try
        {
            var groupIds = await FindGroupIds(userId);
            var canAccess = await _context.ProjectMembers
                .AnyAsync(p =>
                    p.ProjectId == projectId &&
                    (p.RecordId == userId ||
                     groupIds.Contains(p.RecordId)));
            if (!canAccess) return Array.Empty<WorkPackageDto>();

            var packages = await _context.WorkPackages
                .Where(i => i.ProjectId == projectId)
                .AsNoTracking()
                .ToArrayAsync();

            return packages.Select(p => p.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.FindWorkPackages", e);
            return Array.Empty<WorkPackageDto>();
        }
    }

    public async Task<WorkPackageTaskDto[]> FindProjectTasks(Guid userId, Guid projectId)
    {
        try
        {
            var tasks = await (
                from attach in _context.WorkPackageTaskAttachments
                join task in _context.WorkPackageTasks on attach.TaskId equals task.Id
                where attach.UserId == userId && attach.ProjectId == projectId
                select task
            ).AsNoTracking().Distinct().ToArrayAsync();

            return tasks.Select(t => t.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.FindProjectTasks", e);
            return Array.Empty<WorkPackageTaskDto>();
        }
    }

    public async Task<WorkPackageTaskDto[]> FindPackageTasks(Guid userId, Guid packageId)
    {
        try
        {
            var tasks = await (
                from attach in _context.WorkPackageTaskAttachments
                join task in _context.WorkPackageTasks on attach.TaskId equals task.Id
                where attach.UserId == userId && attach.PackageId == packageId
                select task
            ).AsNoTracking().Distinct().ToArrayAsync();

            return tasks.Select(t => t.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.FindPackageTasks", e);
            return Array.Empty<WorkPackageTaskDto>();
        }
    }

    public async Task<ChannelDto[]> FindChannels(Guid userId)
    {
        try
        {
            var groupIds = await FindGroupIds(userId);
            var projectIds = await FindProjectIds(userId, groupIds);
            var packageIds = await FindWorkPackageIds(userId, projectIds, groupIds);
            var allIds = groupIds.Concat(projectIds).Concat(packageIds);
            var channels = await _context.Channels.Where(c => allIds.Contains(c.Id))
                .AsNoTracking()
                .ToArrayAsync();
            return channels.Select(c => c.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.FindChannels", e);
            return Array.Empty<ChannelDto>();
        }
    }

    public async Task<WorkPackageTaskAttachmentDto[]> GetTaskAttachments(Guid taskId, Guid userId, bool byUser)
    {
        try
        {
            var attachments = await (
                from attach in _context.WorkPackageTaskAttachments
                where attach.TaskId == taskId && (
                    (byUser && attach.UserId == userId) || 
                    (!byUser && attach.UserId != userId)
                )
                select attach
            ).AsNoTracking().ToArrayAsync();

            return attachments.Select(a => a.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.GetTaskAttachments", e);
            return Array.Empty<WorkPackageTaskAttachmentDto>();
        }
    }

    public async Task<bool> NewFolder(Guid userId, string directory, string path)
    {
        try
        {
            var id = IncrementalGuid.NewId();
            var when = DateTime.UtcNow;
            var upload = new Upload()
            {
                Directory = directory,
                Extension = ".txt",
                Id = id,
                Name = IGNORE_FILE,
                Path = path,
                Public = false,
                Section = UploadSection.Storage,
                Size = 1,
                Type = FileType.Documents,
                CreatedAt = when,
                RecordId = id,
                UserId = userId
            };
            await _context.Uploads.AddAsync(upload);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.NewFolder", e);
            return false;
        }
    }

    public async Task<UserPlanInfoDto?> GetUserPlan(Guid userId)
    {
        try
        {
            var info = await _context.UserPlanInfo
                .OrderByDescending(i => i.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return info?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.GetUserPlan", e);
            return null;
        }
    }

    public async Task<bool> Store(UploadDto dto)
    {
        try
        {
            var plan = await _context.UserPlanInfo
                .OrderByDescending(i => i.CreatedAt)
                .FirstAsync();
            
            plan.UsedSpace += dto.Size;
            await _context.Uploads.AddAsync(new Upload
            {
                Directory = dto.Directory,
                Extension = dto.Extension,
                CreatedAt = dto.CreatedAt,
                Id = dto.Id,
                Name = dto.Name,
                Path = dto.Path,
                Public = dto.Public,
                Section = dto.Section,
                Size = dto.Size,
                Type = dto.Type,
                RecordId = dto.RecordId,
                UserId = dto.UserId,
                ThumbnailPath = dto.ThumbnailPath,
            });
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.Store", e);
            return false;
        }
    }

    public async Task<UploadDto[]> GetChannelAttachments(Guid channelId, Guid userId, bool byUser)
    {
        try
        {
            if (byUser)
            {
                var userUploads = await (
                    from con in _context.Conversations
                    join upload in _context.Uploads on con.UploadId equals upload.Id
                    where (con.ChannelId == channelId &&
                           con.Type == ConversationType.Upload &&
                           con.UserId == userId)
                    select upload
                ).AsNoTracking().ToArrayAsync();

                return userUploads.Select(u => u.ToDto()).ToArray();
            }
            

            var channel = await _context.Channels
                .AsNoTracking()
                .SingleAsync(c => c.Id == channelId);
            
            Guid[] groupIds;
            bool canAccess = false;
            var exception = new Exception("No Access Exception");
            switch (channel.Type)
            {
                case ChannelType.Bot:
                    throw exception;
                case ChannelType.Direct:
                    throw exception;
                case ChannelType.Group:
                    canAccess = await _context.GroupMembers
                        .AnyAsync(p => p.GroupId == channelId && p.UserId == userId);
                    break;
                case ChannelType.Project:
                    groupIds = await FindGroupIds(userId);
                    canAccess = await _context.ProjectMembers
                        .AnyAsync(p =>
                            p.ProjectId == channelId &&
                            (p.RecordId == userId ||
                             groupIds.Contains(p.RecordId)));
                    break;
                case ChannelType.WorkPackage:
                    groupIds = await FindGroupIds(userId);
                    canAccess = await _context.WorkPackageMembers
                        .AnyAsync(p =>
                            p.PackageId == channelId &&
                            (p.RecordId == userId ||
                             groupIds.Contains(p.RecordId)));
                    break;
            }

            if (!canAccess) throw new Exception();

            var uploads = await (
                from con in _context.Conversations
                join upload in _context.Uploads on con.UploadId equals upload.Id
                where (con.ChannelId == channelId &&
                       con.Type == ConversationType.Upload &&
                       con.UserId != userId)
                select upload
            ).AsNoTracking().ToArrayAsync();

            return uploads.Select(p => p.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageRepository.GetChannelAttachments", e);
            return Array.Empty<UploadDto>();
        }
    }
}