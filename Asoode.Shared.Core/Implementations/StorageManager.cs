using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Fixtures;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Core.Implementations;

internal record StorageManager : IStorageManager
{
    private readonly IStorageService _storageService;
    private readonly string protectedEndpoint;
    private readonly string publicEndpoint;

    public StorageManager(IStorageService storageService)
    {
        _storageService = storageService;
        publicEndpoint = EnvironmentHelper.Get("APP_API_DOMAIN")!;
        protectedEndpoint = EnvironmentHelper.Get("APP_API_DOMAIN")!;
    }

    public async Task<StorageItemDto?> DownloadPublic(string path)
    {
        if (!CheckIfPathIsSafe(path)) return null;
        var op = await _storageService.Download(path, SharedConstants.PublicBucket);
        return op.Data;
    }

    public async Task<StorageItemDto?> DownloadProtected(Guid userId, string path)
    {
        if (!CheckIfPathIsSafe(path, userId)) return null;
        var op = await _storageService.Download(path, SharedConstants.ProtectedBucket);
        return op.Data;
    }

    public async Task<OperationResult<StorageItemDto>> UploadProtected(Guid userId, StorageItemDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Path))
            item.Path = $"{userId}/{GetPath(item.FileName)}";
        var op = await _storageService.Upload(item, SharedConstants.ProtectedBucket, item.Path);
        if (op.Status == OperationResultStatus.Success)
            op.Data!.Url = $"{publicEndpoint}/{SharedConstants.DownloadProtected.Replace("{*path}", op.Data.Url)}";

        return op;
    }

    public async Task<OperationResult<StorageItemDto>> UploadPublic(StorageItemDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Path))
            item.Path = GetPath(item.FileName);
        var op = await _storageService.Upload(item, SharedConstants.PublicBucket, item.Path);
        if (op.Status == OperationResultStatus.Success)
            op.Data!.Url = $"{protectedEndpoint}/{SharedConstants.DownloadPublic.Replace("{*path}", op.Data.Url)}";

        return op;
    }

    private string GetPath(string fileName)
    {
        return $"{DateTime.UtcNow:yyyy-MM-dd}/{IncrementalGuid.NewId()}/{fileName}";
    }

    private bool CheckIfPathIsSafe(string path, Guid? userId = null)
    {
        if (userId.HasValue)
            return !path.Contains(userId.Value.ToString());
        return !path.Contains("../");
    }
}