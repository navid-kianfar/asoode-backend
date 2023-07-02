using System.Text.RegularExpressions;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using StorageItemDto = Asoode.Shared.Abstraction.Dtos.Storage.StorageItemDto;

namespace Asoode.Application.Business.Implementation;

internal record StorageManager : IStorageManager
{
    private const string IGNORE_FILE = "ignore.me.txt";
    private readonly Regex _cleanRegex = new Regex("[/]{2,}", RegexOptions.None);
    private readonly IStorageService _storageService;
    private readonly ILoggerService _loggerService;
    private readonly IStorageRepository _repository;
    private readonly ITranslateService _translateService;
    private readonly string protectedEndpoint;
    private readonly string publicEndpoint;

    public StorageManager(
        ILoggerService loggerService,
        IStorageService storageService,
        IStorageRepository repository,
        ITranslateService translateService
    )
    {
        _storageService = storageService;
        _loggerService = loggerService;
        _repository = repository;
        _translateService = translateService;
        publicEndpoint = EnvironmentHelper.Get("APP_API_DOMAIN")!;
        protectedEndpoint = EnvironmentHelper.Get("APP_API_DOMAIN")!;
    }

    public async Task<StorageItemDto?> DownloadPublic(string path)
    {
        if (!CheckIfPathIsSafe(path)) return null;
        var op = await _storageService.Download(path, SharedConstants.PublicBucket);
        return op.Data;
    }

    public async Task<OperationResult<ExplorerDto>> Mine(Guid userId, FileManagerDto model)
    {
        try
        {
            if (!CheckIfPathIsSafe(model.Path)) return OperationResult<ExplorerDto>.Rejected();
            var result = new ExplorerDto();
            var key = GetUserStoragePath(userId, model.Path);

            var directory = $"/{key}";
            var directories = await _repository.GetDirectories(key, directory);

            var requiredSlashes = directory.Split('/').Length + 1;
            result.Folders = directories
                .GroupBy(i => i.Directory)
                .Where(i => i.Key.Split('/').Length == requiredSlashes)
                .Select(p =>
                {
                    var parent = Path.GetDirectoryName(p.Key)!;
                    return new ExplorerFolderDto
                    {
                        Name = p.Key.Replace(parent, "").Trim('/'),
                        Path = p.Key,
                        Parent = parent,
                        CreatedAt = p.OrderBy(i => i.CreatedAt).First().CreatedAt
                    };
                }).ToArray();

            var files = await _repository.GetFiles(directory);
            result.Files = files.Select(p => p.ToExplorerDto()).ToArray();

            return OperationResult<ExplorerDto>.Success(result);
        }
        catch (Exception ex)
        {
            await _loggerService.Error(ex.Message, "StorageManager.Mine", ex);
            return OperationResult<ExplorerDto>.Failed();
        }
    }

    public Task<OperationResult<ExplorerDto>> SharedByMe(Guid userId, FileManagerDto model)
        => SharedByShared(userId, model, true);

    public Task<OperationResult<ExplorerDto>> SharedByOthers(Guid userId, FileManagerDto model)
        => SharedByShared(userId, model, false);

    private async Task<OperationResult<ExplorerDto>> SharedByShared(Guid userId, FileManagerDto model, bool sharedBy)
    {
        try
        {
            var result = new ExplorerDto();
            var user = await _repository.FindUser(userId);
            var exception = new Exception("No Access Exception");

            if (user == null) throw exception;
            
            if (model.Path == "/")
            {
                result.Folders = new[]
                {
                    new ExplorerFolderDto
                    {
                        Name = _translateService.Get("PROJECTS"),
                        Path = "/projects/",
                        CreatedAt = user.CreatedAt
                    },
                    new ExplorerFolderDto
                    {
                        Name = _translateService.Get("CHANNELS"),
                        Path = "/channels/",
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            else if (model.Path.StartsWith("/projects/"))
            {
                var projects = await _repository.FindProjects(userId);
                result.Folders = projects.Select(p => p.ToExplorerDto()).ToArray();
            }
            else if (model.Path.StartsWith("/project/c/"))
            {
                var guidStr = model.Path.Replace("/project/c/", "");
                var isGuid = Guid.TryParse(guidStr, out Guid id);
                if (!isGuid) throw exception;
                var packages = await _repository.FindWorkPackages(userId, id);
                result.Folders = packages.Select(p => p.ToExplorerDto("c")).ToArray();
            }
            else if (model.Path.StartsWith("/project/s/"))
            {
                var guidStr = model.Path.Replace("/project/s/", "");
                var isGuid = Guid.TryParse(guidStr, out Guid id);
                if (!isGuid) throw exception;

                var tasks = await _repository.FindProjectTasks(userId, id);
                result.Folders = tasks.Select(p =>
                    p.ToExplorerDto($"/project/s/{p.ProjectId}")
                ).ToArray();
            }
            else if (model.Path.StartsWith("/package/"))
            {
                var guidStr = model.Path.Replace("/package/", "");
                var isGuid = Guid.TryParse(guidStr, out Guid id);
                if (!isGuid) throw exception;

                var tasks = await _repository.FindPackageTasks(userId, id);
                result.Folders = tasks.Select(p =>
                    p.ToExplorerDto($"/package/{p.PackageId}")
                ).ToArray();
            }
            else if (model.Path.StartsWith("/task/"))
            {
                var guidStr = model.Path.Replace("/task/", "");
                var isGuid = Guid.TryParse(guidStr, out Guid id);
                if (!isGuid) throw exception;

                var attachments = await _repository.GetTaskAttachments(id, userId, sharedBy);
                result.Files = attachments.Select(p => p.ToExplorerDto()).ToArray();
            }
            else if (model.Path.StartsWith("/channels/"))
            {
                var channels = await _repository.FindChannels(userId);
                result.Folders = channels.Select(p => new ExplorerFolderDto
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
                var uploads = await _repository.GetChannelAttachments(id, userId, sharedBy);
                result.Files = uploads.Select(p => p.ToExplorerDto()).ToArray();
            }
            else
            {
                throw new NotImplementedException();
            }

            return OperationResult<ExplorerDto>.Success(result);
        }
        catch (Exception ex)
        {
            await _loggerService.Error(ex.Message, "StorageManager.SharedBy", ex);
            return OperationResult<ExplorerDto>.Failed();
        }
    }

    public async Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameDto model)
    {
        try
        {
            if (!CheckIfPathIsSafe(model.Path)) return OperationResult<bool>.Rejected();
            var key = $"{GetUserStoragePath(userId, model.Path)}/{model.Name}/{IGNORE_FILE}";
            var op = await _storageService.Upload(
                new MemoryStream(),
                IGNORE_FILE,
                SharedConstants.ProtectedBucket,
                key
            );
            if (op.Status != OperationResultStatus.Success)
                return OperationResult<bool>.Failed();

            var directory = $"/{GetUserStoragePath(userId, model.Path)}/{model.Name}";
            await _repository.NewFolder(userId, directory, op.Data!.Url);
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageManager.NewFolder", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<UploadResultDto>> Upload(Guid userId, StorageItemDto file, FileManagerDto model)
    {
        try
        {
            var plan = await _repository.GetUserPlan(userId);
            if (plan.AttachmentSize < file.FileSize)
            {
                return OperationResult<UploadResultDto>.Success(new UploadResultDto
                {
                    AttachmentSize = true
                });
            }

            if ((plan.UsedSpace + file.FileSize) > plan.Space)
            {
                return OperationResult<UploadResultDto>.Success(new UploadResultDto
                {
                    StorageSize = true
                });
            }

            var path = GetUserStoragePath(userId, model.Path);
            var op = await _storageService.Upload(file, SharedConstants.ProtectedBucket, path);
            if (op.Status != OperationResultStatus.Success)
                return OperationResult<UploadResultDto>.Failed();

            var id = IncrementalGuid.NewId();
            var dto = new UploadDto
            {
                // TODO: fix this
                Path = op.Data!.Url,
                Directory = model.Path,
                ThumbnailPath = string.Empty,
                
                Id = id,
                RecordId = id,
                CreatedAt = DateTime.UtcNow,
                Extension = file.Extension,
                Name = file.FileName,
                Public = false,
                Section = UploadSection.Storage,
                Size = file.FileSize,
                Type = IOHelper.GetFileType(file.Extension),
                UserId = userId,
            };
            await _repository.Store(dto);
            return OperationResult<UploadResultDto>.Success(new UploadResultDto { Success = true });
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageManager.Upload", e);
            return OperationResult<UploadResultDto>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameDto model)
    {
        try
        {
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageManager.Rename", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteDto model)
    {
        try
        {
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageManager.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    private bool CheckIfPathIsSafe(string path, Guid? userId = null)
    {
        if (userId.HasValue)
            return !path.Contains(userId.Value.ToString());
        return !path.Contains("../");
    }

    private string GetUserStoragePath(Guid userId, string path)
    {
        if (path == "/") return CleanPath(Path.Combine("files", userId + path)).TrimEnd('/');

        return CleanPath(path).Trim('/');
    }

    private string CleanPath(string path) => _cleanRegex.Replace(path, "/");
}