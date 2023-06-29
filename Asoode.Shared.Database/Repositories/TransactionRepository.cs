using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class TransactionRepository : ITransactionRepository
{
    public Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model)
    {
        throw new NotImplementedException();
    }
}