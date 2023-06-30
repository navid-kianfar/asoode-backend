using System.Security.Claims;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Logging;
using Asoode.Shared.Abstraction.Dtos.Membership.Authentication;
using Asoode.Shared.Abstraction.Dtos.Membership.Profile;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IAccountService
{
    
    Task<OperationResult<SelectableItem<string>[]>> InviteQuery(Guid userId, AutoCompleteFilter filter);
    Task<OperationResult<Guid?>> ChangeEmail(Guid userId, ChangeEmailDto model);

    Task<OperationResult<bool>> ChangePassword(Guid userId, ChangePasswordDto model);

    Task<OperationResult<Guid?>> ChangePhone(Guid userId, ChangePhoneDto model);

    Task<OperationResult<bool>> ChangePicture(Guid userId, StorageItemDto file);

    Task<OperationResult<bool>> ChangeUsername(Guid userId, UsernameDto model);

    Task<OperationResult<bool>> ConfirmChangeUserVerification(Guid userId,
        ConfirmVerificationDto model);

    Task<OperationResult<LoginResultDto>> ConfirmAccount(ConfirmVerificationDto model);

    ClaimsPrincipal ExtractToken(string token, UserType type = UserType.User);

    Task<OperationResult<ForgetPasswordResultDto>> ForgetPassword(ForgetPasswordDto model);

    Task<OperationResult<UserProfileDto>> GetProfile(Guid userId);

    Task<OperationResult<LoginResultDto>> Login(LoginRequestDto model);

    Task<OperationResult<LoginResultDto>> RecoverPassword(RecoverPasswordDto model);

    Task<OperationResult<RegisterResultDto>> Register(RegisterRequestDto model);

    Task<OperationResult<bool>> RemovePicture(Guid userId);

    Task<OperationResult<bool>> UpdateProfile(Guid userId, UserProfileUpdateDto model, StorageItemDto file);

    Task<OperationResult<bool>> UsernameTaken(UsernameDto model);

    Task<OperationResult<LoginResultDto>> OAuthAuthentication(string email, string firstName,
        string lastName, string marketer);

    Task<OperationResult<GridResult<UserOrderDto>>> Transactions(Guid userId,
        GridFilter filter);

    Task<OperationResult<bool>> ResendVerification(Guid userId, Guid id);
    Task<OperationResult<bool>> AddDevice(Guid userId, PushNotificationDto model);
    Task<OperationResult<DeviceDto[]>> ListDevices(Guid userId);
    Task<OperationResult<bool>> EditDevice(Guid userId, Guid id, TitleDto model);
    Task<OperationResult<bool>> ToggleDevice(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveDevice(Guid userId, Guid id);
    Task<OperationResult<MemberInfoDto>> OtherUserProfile(Guid userId, Guid id);
}