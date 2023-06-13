using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Asoode.Backend.Filters;
using Asoode.Business.Membership;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Membership.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Engine;

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

    protected IFormFile[] ValidateFiles(string[] allowed)
    {
        return Request.Form.Files
            .Where(f => allowed.Contains(Path.GetExtension(f.FileName).ToLower()))
            .ToArray();
    }
}