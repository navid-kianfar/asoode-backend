using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asoode.Servers.Api.Filters
{
    public class ValidateCaptchaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var captchaService = context.HttpContext.RequestServices.GetService<ICaptchaBiz>();
                if (captchaService.Ignore) return;
                var captcha = (ICaptchaChallenge) context.ActionArguments["model"];
                if (captcha != null && captchaService.Validate(captcha)) return;
                context.Result = new JsonResult(OperationResult<object>.Captcha());
            }
            catch
            {
                context.Result = new JsonResult(OperationResult<object>.Captcha());
            }
        }
    }
}