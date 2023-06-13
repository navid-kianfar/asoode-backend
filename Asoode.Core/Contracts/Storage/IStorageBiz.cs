using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Core.Contracts.Storage;

public interface IStorageBiz
{
    Task<OperationResult<ExplorerViewModel>> Mine(Guid userId, FileManagerViewModel model);
    Task<OperationResult<ExplorerViewModel>> SharedByMe(Guid userId, FileManagerViewModel model);
    Task<OperationResult<ExplorerViewModel>> SharedByOthers(Guid userId, FileManagerViewModel model);
    Task<OperationResult<bool>> NewFolder(Guid userId, FileManagerNameViewModel model);
    Task<OperationResult<UploadResultViewModel>> Upload(Guid userId, StorageItemDto file, FileManagerViewModel model);
    Task<OperationResult<bool>> Rename(Guid userId, FileManagerNameViewModel model);
    Task<OperationResult<bool>> Delete(Guid userId, FileManagerDeleteViewModel model);
}