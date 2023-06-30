using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Admin.Business.Implementation;

internal class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILoggerService _loggerService;

    public AccountService(IAccountRepository accountRepository, ILoggerService loggerService)
    {
        _accountRepository = accountRepository;
        _loggerService = loggerService;
    }
    
    public async Task<OperationResult<GridResult<UserDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.List", e);
            return OperationResult<GridResult<UserDto>>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.ResetUserPassword", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> EditUser(Guid userId, Guid id, UserEditDto model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.EditUser", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> ConfirmUser(Guid userId, Guid id)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.ConfirmUser", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> BlockUser(Guid userId, Guid id)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.BlockUser", e);
            return OperationResult<bool>.Failed();
        }
    }
}