using System.Security.Claims;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Core.ViewModels.Membership.Authentication;
using Asoode.Application.Core.ViewModels.Membership.Profile;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.Contracts.Membership;

public interface IAccountBiz
{
    Task<OperationResult<SelectableItem<string>[]>> InviteQuery(Guid userId, AutoCompleteFilter filter);
    Task<OperationResult<Guid?>> ChangeEmail(Guid userId, ChangeEmailViewModel model);

    Task<OperationResult<bool>> ChangePassword(Guid userId, ChangePasswordViewModel model);

    Task<OperationResult<Guid?>> ChangePhone(Guid userId, ChangePhoneViewModel model);

    Task<OperationResult<bool>> ChangePicture(Guid userId, UploadedFileViewModel file);

    Task<OperationResult<bool>> ChangeUsername(Guid userId, UsernameViewModel model);

    Task<OperationResult<bool>> ConfirmChangeUserVerification(Guid userId,
        ConfirmVerificationViewModel model);

    Task<OperationResult<LoginResultViewModel>> ConfirmAccount(ConfirmVerificationViewModel model);

    ClaimsPrincipal ExtractToken(string token, UserType type = UserType.User);

    Task<OperationResult<ForgetPasswordResultViewModel>> ForgetPassword(ForgetPasswordViewModel model);

    Task<OperationResult<UserProfileViewModel>> GetProfile(Guid userId);

    Task<OperationResult<LoginResultViewModel>> Login(LoginRequestViewModel model);

    Task<OperationResult<LoginResultViewModel>> RecoverPassword(RecoverPasswordViewModel model);

    Task<OperationResult<RegisterResultViewModel>> Register(RegisterRequestViewModel model);

    Task<OperationResult<bool>> RemovePicture(Guid userId);

    Task<OperationResult<bool>> UpdateProfile(Guid userId, UserProfileUpdateViewModel model,
        UploadedFileViewModel file);

    Task<OperationResult<bool>> UsernameTaken(UsernameViewModel model);

    Task<OperationResult<LoginResultViewModel>> OAuthAuthentication(string email, string firstName,
        string lastName);

    Task<OperationResult<bool>> ResendVerification(Guid userId, Guid id);
    Task<OperationResult<bool>> AddDevice(Guid userId, PushNotificationViewModel model);
    Task<OperationResult<DeviceViewModel[]>> ListDevices(Guid userId);
    Task<OperationResult<bool>> EditDevice(Guid userId, Guid id, TitleViewModel model);
    Task<OperationResult<bool>> ToggleDevice(Guid userId, Guid id);
    Task<OperationResult<bool>> RemoveDevice(Guid userId, Guid id);
    Task<OperationResult<MemberInfoViewModel>> OtherUserProfile(Guid userId, Guid id);
}