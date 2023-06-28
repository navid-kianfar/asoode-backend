using Asoode.Admin.Abstraction.Contracts;
using Asoode.Admin.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Types;
using UserDto = Asoode.Shared.Abstraction.Dtos.UserDto;

namespace Asoode.Admin.Business.Implementation;

internal class AccountService : IAccountService
{
    public Task<OperationResult<GridResult<UserDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditUser(Guid userId, Guid id, UserEditDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ConfirmUser(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> BlockUser(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}