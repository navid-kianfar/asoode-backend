using Asoode.Shared.Abstraction.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Admin.Server.Controllers;

[JwtAuthorize(UserType.Admin)]
public class BaseController : Controller
{
    
}