using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ForgetPasswordViewModel : ICaptchaChallenge
{
    [Required] public string Username { get; set; }
    public CaptchaChallengeViewModel Captcha { get; set; }
}