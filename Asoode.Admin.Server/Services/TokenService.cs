using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using Microsoft.IdentityModel.Tokens;

namespace Asoode.Admin.Server.Services;

public static class TokenService
{
    private static readonly string _issuer;
    private static readonly string _secret;
    private static readonly byte[] _key;
    private static readonly SymmetricSecurityKey _symetrics;

    static TokenService()
    {
        _issuer = EnvironmentHelper.Get("APP_AUTH_ISSUER")!;
        _secret = EnvironmentHelper.Get("APP_AUTH_SECRET")!;
        _key = Encoding.UTF8.GetBytes(_secret);
        _symetrics = new SymmetricSecurityKey(_key);
    }

    public static TokenValidationParameters GetParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _issuer,
            IssuerSigningKey = _symetrics
        };
    }

    public static string GenerateToken(UserDto user)
    {
        var credentials = new SigningCredentials(_symetrics, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Typ, user.Type.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, IncrementalGuid.NewId().ToString())
        };
        var token = new JwtSecurityToken(_issuer, _issuer, claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);
        return $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
    }

    public static AuthenticatedUserDto? ExtractIdentity(IEnumerable<Claim> claims)
    {
        var all = claims.ToArray();
        if (!all.Any()) return null;
        
        var tokenId = all.FirstOrDefault(i => i.Type == JwtRegisteredClaimNames.Jti)!.Value;
        var userId = all.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)!.Value;
        var userType = all.FirstOrDefault(i => i.Type == JwtRegisteredClaimNames.Typ)!.Value;
        var userName = all.FirstOrDefault(i => i.Type == JwtRegisteredClaimNames.Sub)!.Value;

        var parsed = Enum.TryParse(userType, out UserType parsedType);
        if (!parsed) return null;
        
        return new AuthenticatedUserDto
        {
            UserType = parsedType,
            Username = userName,
            UserId = Guid.Parse(userId),
            TokenId = Guid.Parse(tokenId),
        };
    }
}