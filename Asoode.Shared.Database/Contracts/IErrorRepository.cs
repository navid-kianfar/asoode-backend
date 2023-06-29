using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Error;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IErrorRepository
{
    Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
}