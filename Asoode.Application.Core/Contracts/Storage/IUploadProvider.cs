using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.Contracts.Storage;

public interface IUploadProvider
{
    Task<OperationResult<bool>> Delete(string path, UploadSection section, Guid? userId = null);

    Task<OperationResult<UploadViewModel>> Upload(StoreViewModel model);
    Task<OperationResult<string>> Rename(RenameAttachmentViewModel model);
    Task<OperationResult<UploadViewModel[]>> BulkUpload(StoreViewModel storeViewModel);
    Task<OperationResult<Stream>> BulkDownload(Guid userId, Dictionary<string, string[]> paths);

    Task<OperationResult<bool>> StorageRename(Guid userId, FileManagerNameViewModel model);
    Task<OperationResult<bool>> StorageDelete(Guid userId, FileManagerDeleteViewModel model);
    Task<OperationResult<ExplorerViewModel>> StorageMine(Guid userId, FileManagerViewModel model);
    Task<OperationResult<bool>> StorageNewFolder(Guid userId, FileManagerNameViewModel model);

    Task<OperationResult<UploadResultViewModel>> StorageUpload(Guid userId, UploadedFileViewModel file,
        FileManagerViewModel model);

    Task<OperationResult<ExplorerViewModel>> StorageSharedByMe(Guid userId, FileManagerViewModel model);
    Task<OperationResult<ExplorerViewModel>> StorageSharedByOthers(Guid userId, FileManagerViewModel model);
}