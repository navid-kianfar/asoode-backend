using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Types;
using StorageItemDto = Asoode.Shared.Abstraction.Dtos.StorageItemDto;

namespace Asoode.Shared.Abstraction.Contracts;

public interface IStorageManager
{
    Task<StorageItemDto?> DownloadPublic(string path);
    
    Task<OperationResult<ExplorerDto>> Mine(Guid userId, FileManagerDto model);
    Task<OperationResult<ExplorerDto>> SharedByMe(Guid userId, FileManagerDto model);
    Task<OperationResult<ExplorerDto>> SharedByOthers(Guid userId, FileManagerDto model);
    Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameDto model);
    Task<OperationResult<UploadResultDto>> Upload(Guid userId, StorageItemDto file, FileManagerDto model);
    Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameDto model);
    Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteDto model);
}