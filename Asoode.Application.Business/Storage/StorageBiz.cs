using Asoode.Application.Core.Contracts.Storage;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Business.Storage
{
    internal class StorageBiz : IStorageBiz
    {
        private readonly IUploadProvider _uploadProvider;

        public StorageBiz(IUploadProvider uploadProvider)
        {
            _uploadProvider = uploadProvider;
        }

        public Task<OperationResult<ExplorerViewModel>> Mine(Guid userId, FileManagerViewModel model)
        {
            return _uploadProvider.StorageMine(userId, model);
        }

        public Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameViewModel model)
        {
            return _uploadProvider.StorageNewFolder(userId, model);
        }

        public Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameViewModel model)
        {
            return _uploadProvider.StorageRename(userId, model);
        }

        public Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteViewModel model)
        {
            return _uploadProvider.StorageDelete(userId, model);
        }

        public Task<OperationResult<UploadResultViewModel>> Upload(Guid userId, UploadedFileViewModel file,
            FileManagerViewModel model)
        {
            return _uploadProvider.StorageUpload(userId, file, model);
        }

        public Task<OperationResult<ExplorerViewModel>> SharedByMe(Guid userId, FileManagerViewModel model)
        {
            return _uploadProvider.StorageSharedByMe(userId, model);
        }

        public Task<OperationResult<ExplorerViewModel>> SharedByOthers(Guid userId, FileManagerViewModel model)
        {
            return _uploadProvider.StorageSharedByOthers(userId, model);
        }
    }
}