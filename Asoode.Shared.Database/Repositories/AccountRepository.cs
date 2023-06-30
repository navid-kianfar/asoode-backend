using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _context;
    private readonly ILoggerService _loggerService;

    public AccountRepository(ILoggerService loggerService, AccountDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<OperationResult<GridResult<ExtendedUserDto>>> List(Guid userId,
        GridFilterWithParams<GridQuery> model)
    {
        try
        {
            var query = _context.Users.Where(i =>
                string.IsNullOrEmpty(model.Params.Query) ||
                i.FirstName.Contains(model.Params.Query) ||
                i.LastName.Contains(model.Params.Query) ||
                i.Phone.Contains(model.Params.Query) ||
                i.Email.Contains(model.Params.Query) ||
                i.Bio.Contains(model.Params.Query)
            ).OrderByDescending(i => i.CreatedAt);
            var result = await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) => i.ToDto(startIndex + index + 1)).ToArray();
            }, model.Page, model.PageSize);

            var userIds = result.Data!.Items.Select(i => i.Id).ToArray();
            var plans = await (
                from usr in _context.Users
                join info in _context.UserPlanInfo on usr.Id equals info.UserId
                where userIds.Contains(usr.Id)
                orderby info.CreatedAt descending
                select info
            ).AsNoTracking().ToArrayAsync();

            foreach (var item in result.Data.Items) item.Plan = plans.First(p => p.UserId == item.Id).ToDto();

            return result;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.List", e);
            return OperationResult<GridResult<ExtendedUserDto>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(i => i.Id == id);
            if (user == null) return OperationResult<bool>.NotFound();

            // TODO: fix this
            // HashPassword(user, model.Password);
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.ResetUserPassword", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> EditUser(Guid userId, Guid id, UserEditDto model)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(i => i.Id == id);
            if (user == null) return OperationResult<bool>.NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Type = model.Type;

            if (!string.IsNullOrEmpty(model.Phone))
            {
                user.Phone = model.Phone;
                user.LastPhoneConfirmed = DateTime.UtcNow;
            }
            else
            {
                user.Phone = string.Empty;
                user.LastPhoneConfirmed = null;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
                user.LastEmailConfirmed = DateTime.UtcNow;
            }
            else
            {
                user.Email = string.Empty;
                user.LastEmailConfirmed = null;
            }

            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.EditUser", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ConfirmUser(Guid userId, Guid id)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(i => i.Id == id);
            if (user == null) return OperationResult<bool>.NotFound();
            user.Blocked = false;
            if (!string.IsNullOrEmpty(user.Phone)) user.LastPhoneConfirmed = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(user.Email)) user.LastEmailConfirmed = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.ConfirmUser", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> BlockUser(Guid userId, Guid id)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(i => i.Id == id);
            if (user == null) return OperationResult<bool>.NotFound();
            user.Blocked = true;
            user.LastPhoneConfirmed = null;
            user.LastEmailConfirmed = null;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.BlockUser", e);
            return OperationResult<bool>.Failed();
        }
    }
}