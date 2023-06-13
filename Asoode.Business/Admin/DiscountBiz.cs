using System;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Business.Extensions;
using Asoode.Core.Contracts.Admin;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Asoode.Business.Admin;

internal class DiscountBiz : IDiscountBiz
{
    private readonly IServiceProvider _serviceProvider;

    public DiscountBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<GridResult<DiscountViewModel>>> AdminList(Guid userId, GridFilter model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var query = unit.Discounts.OrderBy(i => i.CreatedAt);
                return await DbHelper.GetPaginatedData(query, tuple =>
                {
                    var (items, startIndex) = tuple;
                    return items.Select((i, index) =>
                    {
                        var vm = i.ToViewModel();
                        vm.Index = startIndex + index + 1;
                        return vm;
                    }).ToArray();
                }, model.Page, model.PageSize);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<GridResult<DiscountViewModel>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> AdminDelete(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                await unit.Discounts.Where(i => i.Id == id).DeleteAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> AdminCreate(Guid userId, DiscountEditableViewModel discount)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                await unit.Discounts.AddAsync(new Discount
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
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> AdminEdit(Guid userId, Guid id, DiscountEditableViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var discount = await unit.Discounts.SingleOrDefaultAsync(i => i.Id == id);
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
                await unit.SaveChangesAsync();
                return OperationResult<bool>.Success(true);
            }
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<bool>.Failed();
        }
    }
}