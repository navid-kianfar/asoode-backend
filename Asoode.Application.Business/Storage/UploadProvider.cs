using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Business.Storage;

internal class UploadProvider : IUploadProvider
{
    public Task<OperationResult<bool>> Delete(string path, UploadSection section, Guid? userId = null)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadViewModel>> Upload(StoreViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<string>> Rename(RenameAttachmentViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadViewModel[]>> BulkUpload(StoreViewModel storeViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Stream>> BulkDownload(Guid userId, Dictionary<string, string[]> paths)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> StorageRename(Guid userId, FileManagerNameViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> StorageDelete(Guid userId, FileManagerDeleteViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ExplorerViewModel>> StorageMine(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> StorageNewFolder(Guid userId, FileManagerNameViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadResultViewModel>> StorageUpload(Guid userId, UploadedFileViewModel file, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ExplorerViewModel>> StorageSharedByMe(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ExplorerViewModel>> StorageSharedByOthers(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }
}