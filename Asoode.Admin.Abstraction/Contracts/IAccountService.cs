using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Admin.Abstraction.Contracts;

public interface IAccountService
{
    Task<OperationResult<GridResult<ExtendedUserDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model);
    Task<OperationResult<bool>> EditUser(Guid userId, Guid id, UserEditDto model);
    Task<OperationResult<bool>> ConfirmUser(Guid userId, Guid id);
    Task<OperationResult<bool>> BlockUser(Guid userId, Guid id);
}