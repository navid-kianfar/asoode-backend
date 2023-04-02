using System.Security.Claims;

namespace Asoode.Application.Business.Membership
{
    public static class AsoodeClaims
    {
        public const string TokenId = JwtRegisteredClaimNames.Jti;
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string Username = JwtRegisteredClaimNames.Sub;
        public const string UserType = "usertype";
    }
}