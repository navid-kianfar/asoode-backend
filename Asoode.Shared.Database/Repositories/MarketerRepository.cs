using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class MarketerRepository : IMarketerRepository
{
    private readonly ILoggerService _loggerService;
    private readonly PremiumDbContext _context;

    public MarketerRepository(ILoggerService loggerService, PremiumDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    public async Task<OperationResult<GridResult<MarketerDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            var query = _context.Marketers
                .Where(i => i.DeletedAt == null)
                .OrderBy(i => i.CreatedAt);
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
            await _loggerService.Error(e.Message, "MarketerRepository.Create", e);
            return OperationResult<GridResult<MarketerDto>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Create(Guid userId, MarketerEditableDto marketer)
    {
        try
        {
            var exists = await _context.Marketers.AnyAsync(i => i.Code == marketer.Code);
            if (exists) return OperationResult<bool>.Duplicate();
            await _context.Marketers.AddAsync(new Marketer
            {
                Code = marketer.Code,
                Description = marketer.Description,
                Fixed = marketer.Fixed,
                Percent = marketer.Percent,
                Title = marketer.Title
            });
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Create", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, MarketerEditableDto model)
    {
        try
        {
            var marketer = await _context.Marketers.SingleAsync(i => i.Id == id);

            marketer.Code = marketer.Code;
            marketer.Description = marketer.Description;
            marketer.Fixed = marketer.Fixed;
            marketer.Percent = marketer.Percent;
            marketer.Title = marketer.Title;
            marketer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Edit", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(Guid userId, Guid id)
    {
        try
        {
            var marketer = await _context.Marketers.SingleAsync(i => i.Id == id);
            marketer.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
    {
        try
        {
            var marketer = await _context.Marketers.SingleAsync(i => i.Id == id);
            marketer.Enabled = !marketer.Enabled;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "MarketerRepository.Toggle", e);
            return OperationResult<bool>.Failed();
        }
    }
}