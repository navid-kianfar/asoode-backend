using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Asoode.Business.Membership
{
    public static class AsoodeClaims
    {
        public const string TokenId = JwtRegisteredClaimNames.Jti;
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string Username = JwtRegisteredClaimNames.Sub;
        public const string UserType = "usertype";
    }
}