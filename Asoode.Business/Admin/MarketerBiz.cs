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

namespace Asoode.Business.Admin;

internal class MarketerBiz : IMarketerBiz
{
    private readonly IServiceProvider _serviceProvider;

    public MarketerBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<GridResult<MarketerViewModel>>> AdminList(Guid userId, GridFilter model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var query = unit.Marketers
                    .Where(i => i.DeletedAt == null)
                    .OrderBy(i => i.CreatedAt);
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
            return OperationResult<GridResult<MarketerViewModel>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> AdminCreate(Guid userId, MarketerEditableViewModel marketer)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var exists = await unit.Marketers.AnyAsync(i => i.Code == marketer.Code);
                if (exists) return OperationResult<bool>.Duplicate();
                await unit.Marketers.AddAsync(new Marketer
                {
                    Code = marketer.Code,
                    Description = marketer.Description,
                    Fixed = marketer.Fixed,
                    Percent = marketer.Percent,
                    Title = marketer.Title
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

    public async Task<OperationResult<bool>> AdminEdit(Guid userId, Guid id, MarketerEditableViewModel model)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var marketer = await unit.Marketers.SingleOrDefaultAsync(i => i.Id == id);

                marketer.Code = marketer.Code;
                marketer.Description = marketer.Description;
                marketer.Fixed = marketer.Fixed;
                marketer.Percent = marketer.Percent;
                marketer.Title = marketer.Title;
                marketer.UpdatedAt = DateTime.UtcNow;

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

    public async Task<OperationResult<bool>> AdminDelete(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var marketer = await unit.Marketers.SingleOrDefaultAsync(i => i.Id == id);
                marketer.DeletedAt = DateTime.UtcNow;
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

    public async Task<OperationResult<bool>> AdminToggle(Guid userId, Guid id)
    {
        try
        {
            using (var unit = _serviceProvider.GetService<GeneralDbContext>())
            {
                var marketer = await unit.Marketers.SingleOrDefaultAsync(i => i.Id == id);
                marketer.Enabled = !marketer.Enabled;
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