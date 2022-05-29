using System.Security.Claims;
using Asoode.Application.Core.Enums.Membership;
using Asoode.Application.Core.Models.Membership;
using Asoode.Servers.Api.Filters;
using Asoode.Servers.Api.Models.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers;

[Localize]
public abstract class BaseController : Controller
{
    private TokenClaimsViewModel _currentUser;

    protected TokenClaimsViewModel Identity
    {
        get
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated) return new TokenClaimsViewModel();
                if (_currentUser == null)
                {
                    Enum.TryParse(User.FindFirstValue(AsoodeClaims.UserId), true, out UserType role);
                    _currentUser = new TokenClaimsViewModel(
                        User.FindFirstValue(AsoodeClaims.Username),
                        Guid.Parse(User.FindFirstValue(AsoodeClaims.UserId)),
                        role
                    );
                }

                return _currentUser;
            }
            catch
            {
                return null;
            }
        }
    }
}