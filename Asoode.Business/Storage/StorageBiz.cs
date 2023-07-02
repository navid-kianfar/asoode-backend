using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Asoode.Business.ProjectManagement;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Extensions;
using Asoode.Core.Helpers;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Storage;

internal class StorageBiz : IStorageBiz
{
    private const string IGNORE_FILE = "ignore.me.txt";
    private readonly Regex _cleanRegex = new Regex("[/]{2,}", RegexOptions.None);
    private readonly IServerInfo _serverInfo;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _remote;
    private readonly string _bucket;

    public StorageBiz(
        IServerInfo serverInfo,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _serverInfo = serverInfo;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _remote = $"https://{_configuration["Setting:Storage:Domain"]}";
        _bucket = _configuration["Setting:Storage:Bucket"];
    }

    public async Task<OperationResult<ExplorerViewModel>> Mine(Guid userId, FileManagerViewModel model)
    {
        try
        {
            if (!IsValidPath(model.Path)) return OperationResult<ExplorerViewModel>.Rejected();
            var result = new ExplorerViewModel();
            var key = GetUserStoragePath(userId, model.Path);
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var directory = $"/{key}";

                var directories = await unit.Uploads.Where(u =>
                        u.Path.Contains(key) &&
                        u.Directory != directory
                    )
                    .AsNoTracking()
                    .ToArrayAsync();

                var requiredSlashes = directory.Split('/').Length + 1;
                result.Folders = directories
                    .GroupBy(i => i.Directory)
                    .Where(i => i.Key.Split('/').Length == requiredSlashes)
                    .Select(p =>
                    {
                        var parent = Path.GetDirectoryName(p.Key);
                        return new ExplorerFolderViewModel
                        {
                            Name = p.Key.Replace(parent, "").Trim('/'),
                            Path = p.Key,
                            Parent = parent,
                            CreatedAt = p.OrderBy(i => i.CreatedAt).First().CreatedAt
                        };
                    }).ToArray();

                var files = await unit.Uploads.Where(u =>
                        u.Directory == directory &&
                        u.Name != IGNORE_FILE
                    )
                    .AsNoTracking()
                    .ToArrayAsync();

                result.Files = files.Select(p =>
                {
                    var ext = Path.GetExtension(p.Path);
                    return new ExplorerFileViewModel
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

            return OperationResult<ExplorerViewModel>.Success(result);
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<ExplorerViewModel>.Failed();
        }
    }

    public async Task<OperationResult<ExplorerViewModel>> SharedByMe(Guid userId, FileManagerViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var translate = _serviceProvider.GetService<ITranslateBiz>();
                var result = new ExplorerViewModel();
                var user = await unit.FindUser(userId);
                var exception = new Exception("No Access Exception");

                if (model.Path == "/")
                {
                    result.Folders = new[]
                    {
                        new ExplorerFolderViewModel
                        {
                            Name = translate.Get("PROJECTS"),
                            Path = "/projects/",
                            CreatedAt = user.CreatedAt
                        },
                        new ExplorerFolderViewModel
                        {
                            Name = translate.Get("CHANNELS"),
                            Path = "/channels/",
                            CreatedAt = user.CreatedAt
                        }
                    };
                }
                else if (model.Path.StartsWith("/projects/"))
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var projects = await unit.FindProjects(userId, groupIds);
                    result.Folders = projects.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = (p.Complex ? "/project/c/" + p.Id : "/project/s/" + p.Id),
                        Parent = "/",
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/project/c/"))
                {
                    var guidStr = model.Path.Replace("/project/c/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var groupIds = await unit.FindGroupIds(userId);
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

                    result.Folders = packages.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/package/" + p.Id,
                        Parent = "/project/c/" + p.ProjectId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/project/s/"))
                {
                    var guidStr = model.Path.Replace("/project/s/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var tasks = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                        where attach.UserId == userId && attach.ProjectId == id
                        select task
                    ).AsNoTracking().Distinct().ToArrayAsync();

                    result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/task/" + p.Id,
                        Parent = "/project/s/" + p.ProjectId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/package/"))
                {
                    var guidStr = model.Path.Replace("/package/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var tasks = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                        where attach.UserId == userId && attach.PackageId == id
                        select task
                    ).AsNoTracking().Distinct().ToArrayAsync();

                    result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/task/" + p.Id,
                        Parent = "/package/" + p.PackageId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/task/"))
                {
                    var guidStr = model.Path.Replace("/task/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var attachments = await (
                        from attach in unit.WorkPackageTaskAttachments
                        where attach.UserId == userId && attach.TaskId == id
                        select attach
                    ).AsNoTracking().ToArrayAsync();

                    result.Files = attachments.Select(p =>
                    {
                        var ext = Path.GetExtension(p.Path);
                        return new ExplorerFileViewModel
                        {
                            Name = p.Title,
                            ExtensionLessName = p.Title,
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
                else if (model.Path.StartsWith("/channels/"))
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var projects = await unit.FindProjectIds(userId, groupIds);
                    var packages = await unit.FindWorkPackageIds(userId, groupIds);
                    var allIds = groupIds.Concat(projects).Concat(packages);
                    var channels = await unit.Channels.Where(c => allIds.Contains(c.Id))
                        .AsNoTracking()
                        .ToArrayAsync();
                    result.Folders = channels.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/channel/" + p.Id,
                        Parent = "/",
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/channel/"))
                {
                    var guidStr = model.Path.Replace("/channel/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var uploads = await (from con in unit.Conversations
                        join upload in unit.Uploads on con.UploadId equals upload.Id
                        where (con.ChannelId == id &&
                               con.Type == ConversationType.Upload &&
                               con.UserId == userId)
                        select upload).AsNoTracking().ToArrayAsync();

                    result.Files = uploads.Select(p =>
                    {
                        var ext = Path.GetExtension(p.Path);
                        return new ExplorerFileViewModel
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
                else
                {
                    throw new NotImplementedException();
                }

                return OperationResult<ExplorerViewModel>.Success(result);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<ExplorerViewModel>.Failed();
        }
    }

    public async Task<OperationResult<ExplorerViewModel>> SharedByOthers(Guid userId, FileManagerViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                var translate = _serviceProvider.GetService<ITranslateBiz>();
                var result = new ExplorerViewModel();
                var user = await unit.FindUser(userId);
                var exception = new Exception("No Access Exception");

                if (model.Path == "/")
                {
                    result.Folders = new[]
                    {
                        new ExplorerFolderViewModel
                        {
                            Name = translate.Get("PROJECTS"),
                            Path = "/projects/",
                            CreatedAt = user.CreatedAt
                        },
                        new ExplorerFolderViewModel
                        {
                            Name = translate.Get("CHANNELS"),
                            Path = "/channels/",
                            CreatedAt = user.CreatedAt
                        }
                    };
                }
                else if (model.Path.StartsWith("/projects/"))
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var projects = await unit.FindProjects(userId, groupIds);
                    result.Folders = projects.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = (p.Complex ? "/project/c/" + p.Id : "/project/s/" + p.Id),
                        Parent = "/",
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/project/c/"))
                {
                    var guidStr = model.Path.Replace("/project/c/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var groupIds = await unit.FindGroupIds(userId);
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

                    result.Folders = packages.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/package/" + p.Id,
                        Parent = "/project/c/" + p.ProjectId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/project/s/"))
                {
                    var guidStr = model.Path.Replace("/project/s/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var groupIds = await unit.FindGroupIds(userId);
                    var canAccess = await unit.ProjectMembers
                        .AnyAsync(p =>
                            p.ProjectId == id &&
                            (p.RecordId == userId ||
                             groupIds.Contains(p.RecordId)));
                    if (!canAccess) throw exception;

                    var tasks = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                        where attach.UserId != userId && attach.ProjectId == id
                        select task
                    ).AsNoTracking().Distinct().ToArrayAsync();

                    result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/task/" + p.Id,
                        Parent = "/project/s/" + p.ProjectId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/package/"))
                {
                    var guidStr = model.Path.Replace("/package/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var groupIds = await unit.FindGroupIds(userId);
                    var canAccess = await unit.WorkPackageMembers
                        .AnyAsync(p =>
                            p.PackageId == id &&
                            (p.RecordId == userId ||
                             groupIds.Contains(p.RecordId)));
                    if (!canAccess) throw exception;

                    var tasks = await (
                        from attach in unit.WorkPackageTaskAttachments
                        join task in unit.WorkPackageTasks on attach.TaskId equals task.Id
                        where attach.UserId != userId && attach.PackageId == id
                        select task
                    ).AsNoTracking().Distinct().ToArrayAsync();

                    result.Folders = tasks.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/task/" + p.Id,
                        Parent = "/package/" + p.PackageId,
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/task/"))
                {
                    var guidStr = model.Path.Replace("/task/", "");
                    var isGuid = Guid.TryParse(guidStr, out Guid id);
                    if (!isGuid) throw exception;

                    var task = await unit.WorkPackageTasks
                        .AsNoTracking()
                        .SingleOrDefaultAsync(t => t.Id == id);

                    var groupIds = await unit.FindGroupIds(userId);
                    var canAccess = await unit.WorkPackageMembers
                        .AnyAsync(p =>
                            p.PackageId == task.PackageId &&
                            (p.RecordId == userId ||
                             groupIds.Contains(p.RecordId)));
                    if (!canAccess) throw exception;

                    var attachments = await (
                        from attach in unit.WorkPackageTaskAttachments
                        where attach.UserId != userId && attach.TaskId == id
                        select attach
                    ).AsNoTracking().ToArrayAsync();

                    result.Files = attachments.Select(p =>
                    {
                        var ext = Path.GetExtension(p.Path);
                        return new ExplorerFileViewModel
                        {
                            Name = p.Title,
                            ExtensionLessName = p.Title,
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
                else if (model.Path.StartsWith("/channels/"))
                {
                    var groupIds = await unit.FindGroupIds(userId);
                    var projects = await unit.FindProjectIds(userId, groupIds);
                    var packages = await unit.FindWorkPackageIds(userId, groupIds);
                    var allIds = groupIds.Concat(projects).Concat(packages);
                    var channels = await unit.Channels.Where(c => allIds.Contains(c.Id))
                        .AsNoTracking()
                        .ToArrayAsync();
                    result.Folders = channels.Select(p => new ExplorerFolderViewModel
                    {
                        Name = p.Title,
                        Path = "/channel/" + p.Id,
                        Parent = "/",
                        CreatedAt = p.CreatedAt
                    }).ToArray();
                }
                else if (model.Path.StartsWith("/channel/"))
                {
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
                        return new ExplorerFileViewModel
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
                else
                {
                    throw new NotImplementedException();
                }

                return OperationResult<ExplorerViewModel>.Success(result);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<ExplorerViewModel>.Failed();
        }
    }

    public async Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                if (!IsValidPath(model.Path)) return OperationResult<bool>.Rejected();
                var key = $"{GetUserStoragePath(userId, model.Path)}/{model.Name}/{IGNORE_FILE}";

                var storageService = _serviceProvider.GetService<IStorageService>()!;
                var op = await storageService.Upload(
                    new StorageItemDto { LocalFile = Path.Combine(_serverInfo.ContentRootPath, IGNORE_FILE) },
                    _bucket,
                    key
                );

                if (op.Status != OperationResultStatus.Success)
                    return OperationResult<bool>.Failed();

                await unit.Uploads.AddAsync(new Upload
                {
                    Directory = $"/{GetUserStoragePath(userId, model.Path)}/{model.Name}",
                    Extension = ".txt",
                    Id = Guid.NewGuid(),
                    Name = IGNORE_FILE,
                    Path = $"{_remote}/{_bucket}/{key}",
                    Public = false,
                    Section = UploadSection.Storage,
                    Size = 1,
                    Type = FileType.Documents,
                    CreatedAt = DateTime.Now,
                    RecordId = Guid.NewGuid(),
                    UserId = userId
                });
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<UploadResultViewModel>> Upload(Guid userId, StorageItemDto file,
        FileManagerViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var plan = await unit.UserPlanInfo.OrderByDescending(i => i.CreatedAt).FirstAsync();
                if (plan.AttachmentSize < file.FileSize)
                {
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        AttachmentSize = true
                    });
                }

                if ((plan.UsedSpace + file.FileSize) > plan.Space)
                {
                    return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                    {
                        StorageSize = true
                    });
                }

                plan.UsedSpace += file.FileSize;
                var result = await Upload(new StoreViewModel
                {
                    FormFile = file,
                    Section = UploadSection.Storage,
                    PlanId = plan.Id,
                    RecordId = userId,
                    UserId = userId,
                    Path = model.Path
                });
                if (result.Status != OperationResultStatus.Success)
                    return OperationResult<UploadResultViewModel>.Rejected();
                await unit.Uploads.AddAsync(new Upload
                {
                    Directory = result.Data.Directory,
                    Extension = result.Data.Extension,
                    Name = result.Data.Name,
                    Path = result.Data.Path,
                    Public = false,
                    Section = UploadSection.Storage,
                    Size = result.Data.Size,
                    RecordId = result.Data.RecordId,
                    ThumbnailPath = result.Data.ThumbnailPath,
                    UserId = result.Data.UserId,
                    Type = result.Data.Type,
                    Id = result.Data.Id
                });
                await unit.SaveChangesAsync();
                return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                {
                    Success = true
                });
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<UploadResultViewModel>.Failed();
        }
    }

    public async Task<OperationResult<UploadViewModel>> Upload(StoreViewModel model)
    {
        try
        {
            long size = 0;
            var source = PrepareStore(model, true);
            var ext = Path.GetExtension(source);

            var storageService = _serviceProvider.GetService<IStorageService>()!;
            var op = await storageService.Upload(
                model.FormFile,
                _bucket,
                source
            );

            var path = $"{_remote}/{_bucket}/{source}";
            return OperationResult<UploadViewModel>.Success(new UploadViewModel
            {
                // TODO: check if its correct
                Path = path,
                Directory = Path.GetDirectoryName($"/{source}"),
                
                Id = Guid.NewGuid(),
                ThumbnailPath = null, //thumbPath,
                Type = IOHelper.GetFileType(ext),
                Size = size,
                Name = Path.GetFileName(source),
                Extension = ext,
                Public = false,
                Section = model.Section,
                CreatedAt = DateTime.Now,
                RecordId = model.RecordId,
                UserId = model.UserId
            });
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return null;
        }
    }

    public Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameViewModel model)
    {
        return Task.FromResult(OperationResult<bool>.Failed());
    }

    public Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteViewModel model)
    {
        return Task.FromResult(OperationResult<bool>.Failed());
    }

    private string PrepareStore(StoreViewModel model, bool storage = false)
    {
        string destinationPath;
        switch (model.Section)
        {
            case UploadSection.Storage:
                var userFolder = Path.Combine("files", model.UserId.ToString());
                var cleanPath = CleanPath((model.Path ?? "/").Trim('/').Replace(userFolder, "")).Trim('/');
                destinationPath = Path.Combine(userFolder, cleanPath);
                break;
            case UploadSection.WorkPackage:
                destinationPath = Path.Combine(
                    model.PlanId.ToString(),
                    "task",
                    model.RecordId.ToString()
                );
                break;
            case UploadSection.Messenger:
                destinationPath = Path.Combine(
                    model.PlanId.ToString(),
                    "channels",
                    model.RecordId.ToString()
                );
                break;
            case UploadSection.Blog:
                destinationPath = Path.Combine(
                    "blog",
                    model.RecordId.ToString()
                );
                break;
            case UploadSection.Pdf:
                destinationPath = Path.Combine(
                    "pdf",
                    model.RecordId.ToString()
                );
                break;
            case UploadSection.UserAvatar:
                destinationPath = Path.Combine(
                    "avatar",
                    model.UserId.ToString()
                );
                break;
            default:
                destinationPath = string.Empty;
                break;
        }

        var name = Path.GetFileNameWithoutExtension(model.FormFile.FileName).RemoveSpecialCharacters();
        var ext = Path.GetExtension(model.FormFile.FileName);
        if (storage) return $"{destinationPath}/{name}{ext}";
        return $"{destinationPath}/{Guid.NewGuid()}/{name}{ext}";
    }

    private async Task<string> SaveTempFile(Stream file, string virtualPath)
    {
        var name = Path.GetFileName(virtualPath);
        var destinationFile = Path.Combine(_serverInfo.FilesRootPath, "tmp", Guid.NewGuid().ToString(), name);
        Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
        using (var stream = new FileStream(destinationFile, FileMode.Create)) await file.CopyToAsync(stream);
        file.Seek(0, SeekOrigin.Begin);
        return destinationFile;
    }

    private bool IsValidPath(string path) => !path.Contains("../");

    private string GetUserStoragePath(Guid userId, string path)
    {
        if (path == "/") return CleanPath(Path.Combine("files", userId + path)).TrimEnd('/');

        return CleanPath(path).Trim('/');
    }

    private string CleanPath(string path) => _cleanRegex.Replace(path, "/");
}