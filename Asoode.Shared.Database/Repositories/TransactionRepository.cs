using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;

namespace Asoode.Shared.Database.Repositories;

internal class TransactionRepository : ITransactionRepository
{
    private readonly PremiumDbContext _context;
    private readonly ILoggerService _loggerService;

    public TransactionRepository(ILoggerService loggerService, PremiumDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            var query = from trans in _context.Transaction
                join order in _context.Orders on trans.OrderId equals order.Id
                join user in _context.Users on order.UserId equals user.Id
                join plan in _context.Plans on order.PlanId equals plan.Id
                where order.Status != OrderStatus.Canceled
                orderby trans.CreatedAt descending
                select new { Transaction = trans, User = user, PlanName = plan.Title };

            return await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) =>
                {
                    var vm = i.Transaction.ToDto();
                    vm.Index = startIndex + index + 1;
                    vm.FullName = i.User.FullName;
                    vm.PlanName = i.PlanName;
                    return vm;
                }).ToArray();
            }, model.Page, model.PageSize);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TransactionRepository.List", e);
            return OperationResult<GridResult<TransactionDto>>.Failed();
        }
    }
}