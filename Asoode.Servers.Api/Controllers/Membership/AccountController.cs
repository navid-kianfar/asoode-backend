using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Membership
{
    [Route("v2/account")]
    [ApiExplorerSettings(GroupName = "Membership")]
    public class AccountController : BaseController
    {
        private readonly IAccountBiz _accountBiz;
        private readonly IServiceProvider _serviceProvider;

        public AccountController(IAccountBiz accountBiz, IServiceProvider serviceProvider)
        {
            _accountBiz = accountBiz;
            _serviceProvider = serviceProvider;
        }

        [JwtAuthorize]
        [HttpPost("invite-query")]
        [ValidateModel]
        public async Task<IActionResult> InviteQuery([FromBody] AutoCompleteFilter filter)
        {
            var op = await _accountBiz.InviteQuery(Identity.UserId, filter);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("transactions")]
        public async Task<IActionResult> Transactions([FromBody] GridFilter model)
        {
            var op = await _accountBiz.Transactions(Identity.UserId, model);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("devices/add")]
        public async Task<IActionResult> AddDevice([FromBody] PushNotificationViewModel model)
        {
            var op = await _accountBiz.AddDevice(Identity.UserId, model);
            return Json(op);
        }
        
        [JwtAuthorize]
        [HttpPost("devices/remove/{id:guid}")]
        public async Task<IActionResult> RemoveDevice(Guid id)
        {
            var op = await _accountBiz.RemoveDevice(Identity.UserId, id);
            return Json(op);
        }
        
        [JwtAuthorize]
        [HttpPost("devices/toggle/{id:guid}")]
        public async Task<IActionResult> ToggleDevice(Guid id)
        {
            var op = await _accountBiz.ToggleDevice(Identity.UserId, id);
            return Json(op);
        }
        
        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("devices/rename/{id:guid}")]
        public async Task<IActionResult> EditDevice(Guid id, [FromBody]TitleViewModel model)
        {
            var op = await _accountBiz.EditDevice(Identity.UserId, id, model);
            return Json(op);
        }
        
        [JwtAuthorize]
        [HttpPost("devices")]
        public async Task<IActionResult> ListDevices()
        {
            var op = await _accountBiz.ListDevices(Identity.UserId);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("email/change")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailViewModel model)
        {
            var op = await _accountBiz.ChangeEmail(Identity.UserId, model);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var op = await _accountBiz.ChangePassword(Identity.UserId, model);
            return Json(op);
        }

        [HttpPost("resend/verification/{id:guid}")]
        public async Task<IActionResult> ResendVerification(Guid id)
        {
            var op = await _accountBiz.ResendVerification(Identity.UserId, id);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("phone/change")]
        public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneViewModel model)
        {
            var op = await _accountBiz.ChangePhone(Identity.UserId, model);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("phone/change/confirm")]
        public async Task<IActionResult> ConfirmChangePhone([FromBody] ConfirmVerificationViewModel model)
        {
            var op = await _accountBiz.ConfirmChangeUserVerification(Identity.UserId, model);
            return Json(op);
        }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("email/change/confirm")]
        public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmVerificationViewModel model)
        {
            var op = await _accountBiz.ConfirmChangeUserVerification(Identity.UserId, model);
            return Json(op);
        }

        // [JwtAuthorize]
        // [HttpPost("avatar/change")]
        // public async Task<IActionResult> ChangePicture()
        // {
        //     var files = ValidateFiles(IOHelper.ImageExtensions);
        //     var op = await _accountBiz.ChangePicture(Identity.UserId, files[0]);
        //     return Json(op);
        // }

        [JwtAuthorize]
        [ValidateModel]
        [HttpPost("username/change")]
        public async Task<IActionResult> ChangeUsername([FromBody] UsernameViewModel model)
        {
            var op = await _accountBiz.ChangeUsername(Identity.UserId, model);
            return Json(op);
        }

        [ValidateModel]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmVerificationViewModel model)
        {
            var op = await _accountBiz.ConfirmAccount(model);
            return Json(op);
        }

        [ValidateModel]
        [ValidateCaptcha]
        [HttpPost("password/forget")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordViewModel model)
        {
            var op = await _accountBiz.ForgetPassword(model);
            return Json(op);
        }

        [ValidateModel]
        [ValidateCaptcha]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestViewModel model)
        {
            var op = await _accountBiz.Login(model);
            return Json(op);
        }

        [JwtAuthorize]
        [HttpPost("profile")]
        public async Task<IActionResult> Profile()
        {
            var op = await _accountBiz.GetProfile(Identity.UserId);
            return Json(op);
        }
        
        [JwtAuthorize]
        [HttpPost("profile/{id:guid}")]
        public async Task<IActionResult> OtherUserProfile(Guid id)
        {
            var op = await _accountBiz.OtherUserProfile(Identity.UserId, id);
            return Json(op);
        }

        [ValidateModel]
        [HttpPost("password/recover")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordViewModel model)
        {
            var op = await _accountBiz.RecoverPassword(model);
            return Json(op);
        }

        [ValidateModel]
        [ValidateCaptcha]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestViewModel model)
        {
            var op = await _accountBiz.Register(model);
            return Json(op);
        }

        // [JwtAuthorize]
        // [HttpPost("avatar/remove")]
        // public async Task<IActionResult> RemovePicture()
        // {
        //     var op = await _accountBiz.RemovePicture(Identity.UserId);
        //     return Json(op);
        // }

        [JwtAuthorize]
        [HttpPost("profile/update")]
        public async Task<IActionResult> UpdateProfile()
        {
            var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
            var file = Request.Form.Files?.FirstOrDefault();
            var model = jsonBiz.Deserialize<UserProfileUpdateViewModel>(Request.Form["data"]);
            var op = await _accountBiz.UpdateProfile(Identity.UserId, model, file);
            return Json(op);
        }

        [ValidateModel]
        [HttpPost("username/taken")]
        public async Task<IActionResult> UsernameTaken([FromBody] UsernameViewModel model)
        {
            var op = await _accountBiz.UsernameTaken(model);
            return Json(op);
        }
    }
}