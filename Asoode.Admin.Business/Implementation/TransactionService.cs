using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Business.Implementation;

internal class TransactionService : ITransactionService
{
    public Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }
}