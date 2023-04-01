using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Import.Trello;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.Contracts.Import;

public interface ITrelloBiz
{
    Task<OperationResult<ProjectPrepareViewModel>> Import(UploadedFileViewModel file, TrelloMapedDataViewModel data,
        Guid userId);
}