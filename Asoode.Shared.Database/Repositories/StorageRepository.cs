using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Communication;
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

    public Task<UserDto> FindUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid[]> FindGroupIds(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectDto[]> FindProjects(Guid userId)
    {
        var groupIds = await _repository.FindGroupIds(userId);
        throw new NotImplementedException();
    }

    public Task<WorkPackageDto[]> FindWorkPackages(Guid userId, Guid projectId)
    {
        var groupIds = await _repository.FindGroupIds(userId);
        var canAccess = await unit.ProjectMembers
            .AnyAsync(p =>
                p.ProjectId == id &&
                (p.RecordId == userId ||
                 groupIds.Contains(p.RecordId)));
        if (!canAccess) throw exception;

        var packages = await unit.WorkPackages
            .AsNoTracking()
            .Where(i => i.ProjectId == id)
            .ToArrayAsync();
    }

    public Task<WorkPackageTaskDto[]> FindProjectTasks(Guid userId, Guid projectId)
    {
        var tasks = await (
            from attach in unit.WorkPackageTaskAttachments
            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
            where attach.UserId == userId && attach.ProjectId == id
            select task
        ).AsNoTracking().Distinct().ToArrayAsync();
    }

    public Task<WorkPackageTaskDto[]> FindPackageTasks(Guid userId, Guid projectId)
    {
        
        var tasks = await (
            from attach in unit.WorkPackageTaskAttachments
            join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
            where attach.UserId == userId && attach.PackageId == id
            select task
        ).AsNoTracking().Distinct().ToArrayAsync();
    }

    public Task<WorkPackageTaskAttachmentDto[]> GetTaskAttachments(Guid result)
    {
        var attachments = await (
            from attach in unit.WorkPackageTaskAttachments
            where attach.UserId == userId && attach.TaskId == id
            select attach
        ).AsNoTracking().ToArrayAsync();
    }

    public Task<ChannelDto[]> FindChannels(Guid userId)
    {
        var groupIds = await _repository.FindGroupIds(userId);
        var projects = await _repository.FindProjectIds(userId, groupIds);
        var packages = await _repository.FindWorkPackageIds(userId, groupIds);
        var allIds = groupIds.Concat(projects).Concat(packages);
        var channels = await unit.Channels.Where(c => allIds.Contains(c.Id))
            .AsNoTracking()
            .ToArrayAsync();
    }

    public Task<WorkPackageTaskAttachmentDto[]> GetTaskAttachments(Guid userId, bool byUser)
    {
        throw new NotImplementedException();
    }

    public Task<UploadDto[]> GetChannelAttachments(Guid userId, bool byUser)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> NewFolder(Guid userId, string directory, string path)
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
            CreatedAt = DateTime.Now,
            RecordId = id,
            UserId = userId
        };
        await _context.Uploads.AddAsync(upload);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<UploadDto[]> GetChannelAttachments(Guid result)
    {
        var uploads = await (from con in unit.Conversations
            join upload in unit.Uploads on con.UploadId equals upload.Id
            where (con.ChannelId == id &&
                   con.Type == ConversationType.Upload &&
                   con.UserId == userId)
            select upload).AsNoTracking().ToArrayAsync();
        
        
        
        
        var guidStr = model.Path.Replace("/channel/", "");
                var isGuid = Guid.TryParse(guidStr, out Guid id);
                if (!isGuid) throw exception;

                var channel = await unit.Channels
                    .AsNoTracking()
                    .SingleOrDefaultAsync(c => c.Id == id);

                Guid[] groupIds;
                bool canAccess = false;
                switch (channel.Type)
                {
                    case ChannelType.Bot:
                        throw exception;
                    case ChannelType.Direct:
                        throw exception;
                    case ChannelType.Group:
                        canAccess = await unit.GroupMembers
                            .AnyAsync(p => p.GroupId == id && p.UserId == userId);
                        break;
                    case ChannelType.Project:
                        groupIds = await unit.FindGroupIds(userId);
                        canAccess = await unit.ProjectMembers
                            .AnyAsync(p =>
                                p.ProjectId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        break;
                    case ChannelType.WorkPackage:
                        groupIds = await unit.FindGroupIds(userId);
                        canAccess = await unit.WorkPackageMembers
                            .AnyAsync(p =>
                                p.PackageId == id &&
                                (p.RecordId == userId ||
                                 groupIds.Contains(p.RecordId)));
                        break;
                }

                if (!canAccess) throw new Exception();

                var uploads = await (from con in unit.Conversations
                    join upload in unit.Uploads on con.UploadId equals upload.Id
                    where (con.ChannelId == id &&
                           con.Type == ConversationType.Upload &&
                           con.UserId != userId)
                    select upload).AsNoTracking().ToArrayAsync();

                result.Files = uploads.Select(p =>
                {
                    var ext = Path.GetExtension(p.Path);
                    return new ExplorerFileDto
                    {
                        Name = p.Name,
                        ExtensionLessName = p.Name,
                        CreatedAt = p.CreatedAt,
                        Extension = ext,
                        Size = 0,
                        Url = p.Path,
                        IsDocument = IOHelper.IsDocument(ext),
                        IsImage = IOHelper.IsImage(ext),
                        IsPdf = IOHelper.IsPdf(ext),
                        IsPresentation = IOHelper.IsPresentation(ext),
                        IsSpreadsheet = IOHelper.IsSpreadsheet(ext),
                        IsArchive = IOHelper.IsArchive(ext),
                        IsExecutable = IOHelper.IsExecutable(ext),
                        IsCode = IOHelper.IsCode(ext),
                        IsOther = IOHelper.IsOther(ext),
                    };
                }).ToArray();
    }
}