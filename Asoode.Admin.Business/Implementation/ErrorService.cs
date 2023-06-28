using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class ErrorService : IErrorService
{
    public Task<OperationResult<GridResult<ErrorDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}