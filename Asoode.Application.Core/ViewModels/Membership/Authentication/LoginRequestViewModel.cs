using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class LoginRequestViewModel : ICaptchaChallenge
{
    [Required] [MinLength(6)] public string Password { get; set; }
    [Required] public string Username { get; set; }
    [Required] public CaptchaChallengeViewModel Captcha { get; set; }
}