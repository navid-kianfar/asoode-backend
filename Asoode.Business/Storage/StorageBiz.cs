using System;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Business.Storage;

internal class StorageBiz : IStorageBiz
{
    public Task<OperationResult<ExplorerViewModel>> Mine(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ExplorerViewModel>> SharedByMe(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ExplorerViewModel>> SharedByOthers(Guid userId, FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadResultViewModel>> Upload(Guid userId, StorageItemDto file,
        FileManagerViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteViewModel model)
    {
        throw new NotImplementedException();
    }
}