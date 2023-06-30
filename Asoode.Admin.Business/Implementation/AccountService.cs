using Asoode.Admin.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Enums;
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

    public Task<OperationResult<GridResult<ExtendedUserDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
        => _accountRepository.List(userId, model);

    public async Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model)
    {
        try
        {
            var op = await _accountRepository.ResetUserPassword(userId, id, model);
            if (op.Status != OperationResultStatus.Success) return op;
            
            // TODO: raise event
            
            return OperationResult<bool>.Success();
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
            var op = await _accountRepository.EditUser(userId, id, model);
            if (op.Status != OperationResultStatus.Success) return op;
            
            // TODO: raise event
            
            return OperationResult<bool>.Success();
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
            var op = await _accountRepository.ConfirmUser(userId, id);
            if (op.Status != OperationResultStatus.Success) return op;
            
            // TODO: raise event
            
            return OperationResult<bool>.Success();
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
            var op = await _accountRepository.BlockUser(userId, id);
            if (op.Status != OperationResultStatus.Success) return op;
            
            // TODO: raise event
            
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.BlockUser", e);
            return OperationResult<bool>.Failed();
        }
    }
}