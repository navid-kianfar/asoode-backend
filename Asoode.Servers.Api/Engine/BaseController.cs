using System.Security.Claims;
using Asoode.Application.Business.Membership;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.Membership.Authentication;
using Asoode.Servers.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Engine
{
    public abstract class BaseController : Controller
    {
        private TokenClaimsViewModel? _currentUser;

        protected TokenClaimsViewModel? Identity
        {
            get
            {
                try
                {
                    if (!(User.Identity?.IsAuthenticated ?? false)) return new TokenClaimsViewModel();
                    if (_currentUser == null)
                    {
                        Enum.TryParse(User.FindFirstValue(AsoodeClaims.UserId), true, out UserType role);
                        _currentUser = new TokenClaimsViewModel(
                            User.FindFirstValue(AsoodeClaims.Username)!,
                            Guid.Parse(User.FindFirstValue(AsoodeClaims.UserId)!),
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
}