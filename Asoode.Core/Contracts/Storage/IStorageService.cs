using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Core.Contracts.Storage;

public interface IStorageService
{
    Task<OperationResult<UploadedStorageItemDto>> Upload(
        StorageItemDto file,
        string bucketName,
        string path
    );

    Task<OperationResult<UploadedStorageItemDto>> Upload(
        StorageItemDto file
    );

    Task<OperationResult<StorageItemDto>> Download(string path, string bucketName);
    Task<OperationResult<StorageItemDto>> Download(string path);

    Task<OperationResult<bool>> Delete(string path, string bucketName);
    Task<OperationResult<bool>> Exists(string path, string bucketName);

    Task<OperationResult<bool>> Delete(string path);
    Task<OperationResult<bool>> Exists(string path);
    Task<OperationResult<Stream>> BulkDownload(Dictionary<string, string[]> paths);
    Task<OperationResult<string>> Rename(string path, string fileName);
    Task<OperationResult<UploadViewModel[]>> BulkUpload(StorageItemDto file);
}