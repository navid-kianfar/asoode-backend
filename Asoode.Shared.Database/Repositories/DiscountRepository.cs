using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class DiscountRepository : IDiscountRepository
{
    private readonly PremiumDbContext _context;
    private readonly ILoggerService _loggerService;

    public DiscountRepository(ILoggerService loggerService, PremiumDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, DiscountEditableDto model)
    {
        try
        {
            var discount = await _context.Discounts.SingleAsync(i => i.Id == id);
            discount.Code = model.Code;
            discount.Description = model.Description;
            discount.Percent = model.Percent;
            discount.Title = model.Title;
            discount.Unit = model.Unit;
            discount.EndAt = model.EndAt;
            discount.ForUser = model.ForUser;
            discount.MaxUnit = model.MaxUnit;
            discount.MaxUsage = model.MaxUsage;
            discount.StartAt = model.StartAt;
            discount.PlanId = model.PlanId;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.Edit", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Create(Guid userId, DiscountEditableDto discount)
    {
        try
        {
            await _context.Discounts.AddAsync(new Discount
            {
                Code = discount.Code,
                Description = discount.Description,
                Percent = discount.Percent,
                Title = discount.Title,
                Unit = discount.Unit,
                EndAt = discount.EndAt,
                ForUser = discount.ForUser,
                MaxUnit = discount.MaxUnit,
                MaxUsage = discount.MaxUsage,
                StartAt = discount.StartAt,
                PlanId = discount.PlanId
            });
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.Create", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        try
        {
            await _context.Discounts.Where(i => i.Id == id).ExecuteDeleteAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<DiscountDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            var query = _context.Discounts.OrderBy(i => i.CreatedAt);
            return await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) =>
                {
                    var vm = i.ToDto();
                    vm.Index = startIndex + index + 1;
                    return vm;
                }).ToArray();
            }, model.Page, model.PageSize);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "DiscountRepository.List", e);
            return OperationResult<GridResult<DiscountDto>>.Failed();
        }
    }
}