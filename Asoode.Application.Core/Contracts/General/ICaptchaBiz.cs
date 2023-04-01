using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.Contracts.General;

public interface ICaptchaBiz
{
    bool Ignore { get; set; }
    OperationResult<CaptchaRequestViewModel> Generate();
    bool Validate(ICaptchaChallenge captcha);
}