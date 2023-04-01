using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.Contracts.General;

public interface ICaptchaChallenge
{
    CaptchaChallengeViewModel Captcha { get; set; }
}