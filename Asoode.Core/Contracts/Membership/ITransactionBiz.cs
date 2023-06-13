using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Transaction;

namespace Asoode.Core.Contracts.Membership;

public interface ITransactionBiz
{
    Task<OperationResult<GridResult<TransactionViewModel>>> AdminTransactionsList(Guid userId, GridFilter model);
}