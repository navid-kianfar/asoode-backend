using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IMarketerRepository
{
    Task<OperationResult<GridResult<MarketerDto>>> List(Guid userId, GridFilter model);
    Task<OperationResult<bool>> Create(Guid userId, MarketerEditableDto model);
    Task<OperationResult<bool>> Edit(Guid userId, Guid id, MarketerEditableDto model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
    Task<OperationResult<bool>> Toggle(Guid userId, Guid id);
}