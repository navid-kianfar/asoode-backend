using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Error;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Abstraction.Contracts;

public interface IErrorService
{
    Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
}