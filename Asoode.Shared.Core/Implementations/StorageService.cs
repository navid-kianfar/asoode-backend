using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using HeyRed.Mime;
using Minio;

namespace Asoode.Shared.Core.Implementations;

internal record StorageService : IStorageService, IDisposable
{
    private readonly MinioClient _client;
    private readonly ILoggerService _loggerService;

    public StorageService(ILoggerService loggerService)
    {
        var endpoint = EnvironmentHelper.Get("APP_STORAGE_SERVER")!;
        var accessKey = EnvironmentHelper.Get("APP_STORAGE_USER")!;
        var secretKey = EnvironmentHelper.Get("APP_STORAGE_PASS")!;
        var port = int.Parse(EnvironmentHelper.Get("APP_STORAGE_PORT")!);
        _loggerService = loggerService;
        _client = new MinioClient()
            .WithEndpoint(endpoint, port)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public Task<OperationResult<StorageItemDto>> Upload(StorageItemDto file, string bucketName, string path)
    {
        return Upload(file.Stream!, file.FileName, bucketName, path);
    }

    public async Task<OperationResult<StorageItemDto>> Upload(Stream stream, string fileName, string bucketName,
        string path)
    {
        try
        {
            bucketName = FixBucketName(bucketName);
            
            await EnsureBucketExists(_client, bucketName, true);

            var destinationPath = path;
            if (!destinationPath.EndsWith(path))
                destinationPath = Path.Combine(destinationPath, fileName);

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(destinationPath)
                .WithObjectSize(stream.Length)
                .WithStreamData(stream);

            await _client.PutObjectAsync(args);

            var ext = Path.GetExtension(fileName);

            return OperationResult<StorageItemDto>.Success(new StorageItemDto
            {
                Url = destinationPath,
                Extension = ext,
                FileName = fileName,
                FileSize = stream.Length,
                MimeType = MimeTypesMap.GetMimeType(ext),
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Upload", e);
            return OperationResult<StorageItemDto>.Failed();
        }
    }

    public async Task<OperationResult<StorageItemDto>> Download(string path, string bucketName)
    {
        try
        {
            bucketName = FixBucketName(bucketName);
            var stream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(path)
                .WithCallbackStream(objectStream =>
                {
                    objectStream.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                });
            var response = await _client.GetObjectAsync(args);
            if (response == null) return OperationResult<StorageItemDto>.Failed();

            return OperationResult<StorageItemDto>.Success(new StorageItemDto
            {
                Url = path,
                Stream = stream,
                CreatedAt = DateTime.UtcNow,
                Extension = Path.GetExtension(path),
                FileName = Path.GetFileName(path),
                FileSize = response.Size,
                MimeType = response.ContentType
            });
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Download", e);
            return OperationResult<StorageItemDto>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(string path, string bucketName)
    {
        try
        {
            bucketName = FixBucketName(bucketName);
            var args = new RemoveObjectArgs().WithBucket(bucketName).WithObject(path);
            await _client.RemoveObjectAsync(args);
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Exists(string path, string bucketName)
    {
        try
        {
            bucketName = FixBucketName(bucketName);
            // Check whether the object exists using StatObjectAsync(). If the object is not found,
            // StatObjectAsync() will throw an exception.
            var args = new StatObjectArgs().WithBucket(bucketName).WithObject(path);
            await _client.StatObjectAsync(args);
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            if (e.Message == "MinIO API responded with message=Not found.")
                return OperationResult<bool>.Success();
            await _loggerService.Error(e.Message, "StorageService.Exists", e);
            return OperationResult<bool>.Failed();
        }
    }

    private async Task<bool> EnsureBucketExists(MinioClient client, string bucketName, bool force)
    {
        bucketName = FixBucketName(bucketName);
        var exists = await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exists || !force) return exists;
        await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        return true;
    }

    private string FixBucketName(string bucketName)
    {
        return bucketName.ToLower().Trim().Replace(" ", "_");
    }
}