using Asoode.Application.Core.Primitives.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Servers.Api.Filters
{
    public class JwtAuthorizeAttribute : TypeFilterAttribute
    {
        public JwtAuthorizeAttribute(UserType accountType = UserType.User)
            : base(typeof(JwtAuthorize))
        {
            Arguments = new object[] {accountType};
        }
    }
}