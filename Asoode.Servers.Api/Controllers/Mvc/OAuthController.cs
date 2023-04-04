using System.Security.Claims;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Membership;
using Asoode.Application.Core.Helpers;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Servers.Api.Engine;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Controllers.Mvc
{
    [Route("oauth")]
    public class OAuthController : BaseController
    {
        private const string LocalServer = "http://localhost:4200";
        private readonly IServiceProvider _serviceProvider;

        public OAuthController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            string returnUrl = "panel";
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/oauth/google-callback?returnUrl={returnUrl}";
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties {RedirectUri = redirectUrl}
            );
        }
        
        [Route("google-login/admin")]
        public IActionResult AdminGoogleLogin()
        {
            string returnUrl = "admin";
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/oauth/google-callback?returnUrl={returnUrl}";
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties {RedirectUri = redirectUrl}
            );
        }

        [Route("google-callback")]
        public async Task GoogleCallback(string returnUrl = "panel", string remoteError = null)
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var panel = returnUrl == "panel";
            var env = _serviceProvider.GetService<IWebHostEnvironment>();
            var remoteServer = $"https://panel.{configuration["Setting:Domain"]}";
            var adminRemoteServer = $"https://admin.{configuration["Setting:Domain"]}";
            var ret = env.IsDevelopment() ? LocalServer : (panel ? remoteServer : adminRemoteServer);

            if (remoteError != null)
            {
                Response.Redirect(ret);
                return;
            }

            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (info == null || !info.Succeeded)
            {
                Response.Redirect(ret);
                return;
            }

            if (!info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Response.Redirect(ret);
                return;
            }

            var lastName = "";
            var firstName = "";
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
            {
                lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            }

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
            {
                firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            }
            
            var result = await _serviceProvider.GetService<IAccountBiz>()!.OAuthAuthentication(email, firstName, lastName);

            if (result.Status != OperationResultStatus.Success)
            {
                Response.Redirect(ret);
                return;
            }

            var access = _serviceProvider.GetService<IJsonService>()!.Serialize(new
            {
                result.Data.Token,
                result.Data.Username,
                result.Data.UserId,
            });
            var encoded = CryptoHelper.Base64Encode(access);
            Response.Redirect($"{ret}/login?access={encoded}");
        }
    }
}