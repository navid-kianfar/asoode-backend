using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface ITransactionRepository
{
    Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model);
}