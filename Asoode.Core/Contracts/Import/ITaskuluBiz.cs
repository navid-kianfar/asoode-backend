using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Core.Contracts.Import;

public interface ITaskuluBiz
{
    Task<OperationResult<ProjectPrepareViewModel>> Import(StorageItemDto file, Guid userId);
}