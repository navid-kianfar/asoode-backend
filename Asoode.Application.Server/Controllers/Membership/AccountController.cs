using Microsoft.AspNetCore.Mvc;

namespace Asoode.Application.Server.Controllers.Membership;

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

    
    [HttpPost("invite-query")]
    
    public async Task<IActionResult> InviteQuery([FromBody] AutoCompleteFilter filter)
    {
        var op = await _accountBiz.InviteQuery(_identity.User!.UserId, filter);
        return Json(op);
    }

    
    
    [HttpPost("transactions")]
    public async Task<IActionResult> Transactions([FromBody] GridFilter model)
    {
        var op = await _accountBiz.Transactions(_identity.User!.UserId, model);
        return Json(op);
    }

    
    
    [HttpPost("devices/add")]
    public async Task<IActionResult> AddDevice([FromBody] PushNotificationDto model)
    {
        var op = await _accountBiz.AddDevice(_identity.User!.UserId, model);
        return Json(op);
    }

    
    [HttpPost("devices/remove/{id:guid}")]
    public async Task<IActionResult> RemoveDevice(Guid id)
    {
        var op = await _accountBiz.RemoveDevice(_identity.User!.UserId, id);
        return Json(op);
    }

    
    [HttpPost("devices/toggle/{id:guid}")]
    public async Task<IActionResult> ToggleDevice(Guid id)
    {
        var op = await _accountBiz.ToggleDevice(_identity.User!.UserId, id);
        return Json(op);
    }

    
    
    [HttpPost("devices/rename/{id:guid}")]
    public async Task<IActionResult> EditDevice(Guid id, [FromBody] TitleDto model)
    {
        var op = await _accountBiz.EditDevice(_identity.User!.UserId, id, model);
        return Json(op);
    }

    
    [HttpPost("devices")]
    public async Task<IActionResult> ListDevices()
    {
        var op = await _accountBiz.ListDevices(_identity.User!.UserId);
        return Json(op);
    }

    
    
    [HttpPost("email/change")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
    {
        var op = await _accountBiz.ChangeEmail(_identity.User!.UserId, model);
        return Json(op);
    }

    
    
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        var op = await _accountBiz.ChangePassword(_identity.User!.UserId, model);
        return Json(op);
    }

    [HttpPost("resend/verification/{id:guid}")]
    public async Task<IActionResult> ResendVerification(Guid id)
    {
        var op = await _accountBiz.ResendVerification(_identity.User!.UserId, id);
        return Json(op);
    }

    
    
    [HttpPost("phone/change")]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneDto model)
    {
        var op = await _accountBiz.ChangePhone(_identity.User!.UserId, model);
        return Json(op);
    }

    
    
    [HttpPost("phone/change/confirm")]
    public async Task<IActionResult> ConfirmChangePhone([FromBody] ConfirmVerificationDto model)
    {
        var op = await _accountBiz.ConfirmChangeUserVerification(_identity.User!.UserId, model);
        return Json(op);
    }

    
    
    [HttpPost("email/change/confirm")]
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmVerificationDto model)
    {
        var op = await _accountBiz.ConfirmChangeUserVerification(_identity.User!.UserId, model);
        return Json(op);
    }

    // 
    // [HttpPost("avatar/change")]
    // public async Task<IActionResult> ChangePicture()
    // {
    //     var files = ValidateFiles(IOHelper.ImageExtensions);
    //     var op = await _accountBiz.ChangePicture(_identity.User!.UserId, files[0]);
    //     return Json(op);
    // }

    
    
    [HttpPost("username/change")]
    public async Task<IActionResult> ChangeUsername([FromBody] UsernameDto model)
    {
        var op = await _accountBiz.ChangeUsername(_identity.User!.UserId, model);
        return Json(op);
    }

    
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmVerificationDto model)
    {
        var op = await _accountBiz.ConfirmAccount(model);
        return Json(op);
    }

    
    [HttpPost("password/forget")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto model)
    {
        var op = await _accountBiz.ForgetPassword(model);
        return Json(op);
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        var op = await _accountBiz.Login(model);
        return Json(op);
    }

    
    [HttpPost("profile")]
    public async Task<IActionResult> Profile()
    {
        var op = await _accountBiz.GetProfile(_identity.User!.UserId);
        return Json(op);
    }

    
    [HttpPost("profile/{id:guid}")]
    public async Task<IActionResult> OtherUserProfile(Guid id)
    {
        var op = await _accountBiz.OtherUserProfile(_identity.User!.UserId, id);
        return Json(op);
    }

    
    [HttpPost("password/recover")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto model)
    {
        var op = await _accountBiz.RecoverPassword(model);
        return Json(op);
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
    {
        var op = await _accountBiz.Register(model);
        return Json(op);
    }

    // 
    // [HttpPost("avatar/remove")]
    // public async Task<IActionResult> RemovePicture()
    // {
    //     var op = await _accountBiz.RemovePicture(_identity.User!.UserId);
    //     return Json(op);
    // }

    
    [HttpPost("profile/update")]
    public async Task<IActionResult> UpdateProfile()
    {
        var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
        var file = await Request.Form.Files.First().ToStorageItem();
        var model = jsonBiz.Deserialize<UserProfileUpdateDto>(Request.Form["data"]);
        var op = await _accountBiz.UpdateProfile(_identity.User!.UserId, model, file);
        return Json(op);
    }

    
    [HttpPost("username/taken")]
    public async Task<IActionResult> UsernameTaken([FromBody] UsernameDto model)
    {
        var op = await _accountBiz.UsernameTaken(model);
        return Json(op);
    }
}