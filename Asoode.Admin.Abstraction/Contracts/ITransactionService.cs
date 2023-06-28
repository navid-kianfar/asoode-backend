using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Abstraction.Contracts;

public interface ITransactionService
{
    Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model);
}