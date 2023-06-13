using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Storage;
using HeyRed.Mime;
using Microsoft.Extensions.Configuration;
using Minio;

namespace Asoode.Business.Storage;

internal record StorageService : IStorageService, IDisposable
{
    private readonly string _defaultEndpointPrefix;
    private readonly string _defaultBucketName;
    private readonly MinioClient _client;
    private readonly ILoggerService _loggerServiceService;

    public StorageService(ILoggerService loggerServiceService, IConfiguration configuration)
    {
        var port = int.Parse(configuration["Setting:Storage:Port"]!);
        var endpoint = configuration["Setting:Storage:Server"]!;
        var accessKey = configuration["Setting:Storage:AccessKey"]!;
        var secretKey = configuration["Setting:Storage:SecretKey"]!;
        _defaultBucketName = configuration["Setting:Storage:Bucket"]!;
        _defaultEndpointPrefix = $"{configuration["Setting:Endpoint"]!}/v2/storage/download";

        _loggerServiceService = loggerServiceService;
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

    public Task<OperationResult<UploadedStorageItemDto>> Upload(StorageItemDto file, string bucketName, string path)
    {
        return Upload(file.Stream!, file.FileName, bucketName, path);
    }

    public async Task<OperationResult<UploadedStorageItemDto>> Upload(StorageItemDto file)
    {
        if (!string.IsNullOrEmpty(file.LocalFile))
        {
            if (!File.Exists(file.LocalFile)) return OperationResult<UploadedStorageItemDto>.NotFound();

            await using (var fileStream = new FileStream(file.LocalFile, FileMode.Open))
            {
                file.Stream = new MemoryStream();
                await fileStream.CopyToAsync(file.Stream);
                file.Stream.Seek(0, SeekOrigin.Begin);
            }

            file.FileSize = file.Stream.Length;
            file.FileName = Path.GetFileName(file.LocalFile);
            file.Extension = Path.GetExtension(file.LocalFile);
            file.MimeType = MimeTypesMap.GetMimeType(file.Extension);
        }

        return await Upload(file.Stream!, file.FileName, _defaultBucketName, file.GetPath());
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
            await _loggerServiceService.Error(e);
            return OperationResult<StorageItemDto>.Failed();
        }
    }

    public Task<OperationResult<StorageItemDto>> Download(string path)
    {
        return Download(path, _defaultBucketName);
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
            await _loggerServiceService.Error(e);
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
            await _loggerServiceService.Error(e);
            return OperationResult<bool>.Failed();
        }
    }

    public Task<OperationResult<bool>> Delete(string path)
    {
        return Delete(path, _defaultBucketName);
    }

    public Task<OperationResult<bool>> Exists(string path)
    {
        return Exists(path, _defaultBucketName);
    }


    public Task<OperationResult<string>> Rename(string path, string fileName)
    {
        var op = OperationResult<string>.Failed();
        // TODO: FIX THIS
        return Task.FromResult(op);
    }

    public Task<OperationResult<Stream>> BulkDownload(Dictionary<string, string[]> paths)
    {
        var op = OperationResult<Stream>.Failed();
        // TODO: FIX THIS
        return Task.FromResult(op);
    }

    public Task<OperationResult<UploadViewModel[]>> BulkUpload(StorageItemDto file)
    {
        var op = OperationResult<UploadViewModel[]>.Failed();
        // TODO: FIX THIS
        return Task.FromResult(op);
    }


    private async Task<OperationResult<UploadedStorageItemDto>> Upload(Stream stream, string fileName,
        string bucketName,
        string path)
    {
        try
        {
            bucketName = FixBucketName(bucketName);
            await EnsureBucketExists(_client, bucketName, true);
            
            var destinationPath = path;
            if (!destinationPath.EndsWith(fileName))
                destinationPath = Path.Combine(destinationPath, fileName);

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(destinationPath)
                .WithObjectSize(stream.Length)
                .WithStreamData(stream);

            await _client.PutObjectAsync(args);

            var ext = Path.GetExtension(fileName);

            return OperationResult<UploadedStorageItemDto>.Success(new UploadedStorageItemDto
            {
                Extension = ext,
                FileName = fileName,
                FileSize = stream.Length,
                MimeType = MimeTypesMap.GetMimeType(ext),
                CreatedAt = DateTime.UtcNow,
                Url = $"{_defaultEndpointPrefix}/{destinationPath}",
            });
        }
        catch (Exception e)
        {
            await _loggerServiceService.Error(e);
            return OperationResult<UploadedStorageItemDto>.Failed();
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