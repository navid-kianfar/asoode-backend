using Asoode.Core.Primitives.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Asoode.Backend.Filters
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