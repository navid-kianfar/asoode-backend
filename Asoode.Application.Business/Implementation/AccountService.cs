using System.Security.Claims;
using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Logging;
using Asoode.Shared.Abstraction.Dtos.Membership.Authentication;
using Asoode.Shared.Abstraction.Dtos.Membership.Profile;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class AccountService : IAccountService
{
    public Task<OperationResult<SelectableItem<string>[]>> InviteQuery(Guid userId, AutoCompleteFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Guid?>> ChangeEmail(Guid userId, ChangeEmailDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangePassword(Guid userId, ChangePasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Guid?>> ChangePhone(Guid userId, ChangePhoneDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangePicture(Guid userId, StorageItemDto file)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ChangeUsername(Guid userId, UsernameDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ConfirmChangeUserVerification(Guid userId, ConfirmVerificationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<LoginResultDto>> ConfirmAccount(ConfirmVerificationDto model)
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal ExtractToken(string token, UserType type = UserType.User)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ForgetPasswordResultDto>> ForgetPassword(ForgetPasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UserProfileDto>> GetProfile(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<LoginResultDto>> Login(LoginRequestDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<LoginResultDto>> RecoverPassword(RecoverPasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<RegisterResultDto>> Register(RegisterRequestDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemovePicture(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> UpdateProfile(Guid userId, UserProfileUpdateDto model, StorageItemDto file)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> UsernameTaken(UsernameDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<LoginResultDto>> OAuthAuthentication(string email, string firstName, string lastName, string marketer)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<GridResult<UserOrderDto>>> Transactions(Guid userId, GridFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ResendVerification(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> AddDevice(Guid userId, PushNotificationDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<DeviceDto[]>> ListDevices(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> EditDevice(Guid userId, Guid id, TitleDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ToggleDevice(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> RemoveDevice(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<MemberInfoDto>> OtherUserProfile(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}