using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Membership.Transaction;

namespace Asoode.Application.Core.Contracts.Membership;

public interface ITransactionBiz
{
    Task<OperationResult<GridResult<TransactionViewModel>>> AdminTransactionsList(Guid userId, GridFilter model);
}