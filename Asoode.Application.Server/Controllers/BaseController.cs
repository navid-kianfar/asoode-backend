using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Endpoint.Extensions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asoode.Application.Server.Controllers;

[Authorize]
public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var extracted = TokenService.ExtractIdentity(context.HttpContext.User.Claims);
        if (extracted == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var identity = context.HttpContext.RequestServices.GetService<IUserIdentityService>()!;
        identity.IsAuthenticated = true;
        identity.User = extracted;
        base.OnActionExecuting(context);
    }
}