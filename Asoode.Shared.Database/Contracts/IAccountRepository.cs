using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface IAccountRepository
{
    Task<OperationResult<bool>> BlockUser(Guid userId, Guid id);
    Task<OperationResult<bool>> ConfirmUser(Guid userId, Guid id);
    Task<OperationResult<bool>> EditUser(Guid userId, Guid id, UserEditDto model);
    Task<OperationResult<GridResult<ExtendedUserDto>>> List(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<bool>> ResetUserPassword(Guid userId, Guid id, UserResetPasswordDto model);
}