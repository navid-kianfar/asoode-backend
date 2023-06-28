using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class MarketerService : IMarketerService
{
    public Task<OperationResult<GridResult<MarketerDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Create(Guid userId, MarketerEditableDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Edit(Guid userId, Guid id, MarketerEditableDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}