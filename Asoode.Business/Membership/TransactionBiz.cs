using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Membership.Transaction;
using Asoode.Data.Contexts;
using Asoode.Data.Models.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Membership;

internal class TransactionBiz : ITransactionBiz
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<GridResult<TransactionViewModel>>> AdminTransactionsList(Guid userId,
        GridFilter model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<AccountDbContext>())
            {
                var query = from trans in unit.Transaction
                    join order in unit.Orders on trans.OrderId equals order.Id
                    join user in unit.Users on order.UserId equals user.Id
                    join plan in unit.Plans on order.PlanId equals plan.Id
                    where order.Status != OrderStatus.Canceled
                    orderby trans.CreatedAt descending
                    select new { Transaction = trans, User = user, PlanName = plan.Title };

                return await DbHelper.GetPaginatedData(query, tuple =>
                {
                    var (items, startIndex) = tuple;
                    return items.Select((i, index) =>
                    {
                        var vm = i.Transaction.ToViewModel();
                        vm.Index = startIndex + index + 1;
                        vm.FullName = i.User.FullName;
                        vm.PlanName = i.PlanName;
                        return vm;
                    }).ToArray();
                }, model.Page, model.PageSize);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<GridResult<TransactionViewModel>>.Failed();
        }
    }
}