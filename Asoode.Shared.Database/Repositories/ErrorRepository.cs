using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Error;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class ErrorRepository : IErrorRepository
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