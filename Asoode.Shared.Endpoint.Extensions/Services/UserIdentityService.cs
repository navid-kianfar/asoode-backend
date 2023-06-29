using System.Security.Claims;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Shared.Endpoint.Extensions.Services;

public class UserIdentityService : IUserIdentityService
{
    public bool IsAuthenticated { get; set; }
    public AuthenticatedUserDto? User { get; set; }

    public bool TryAuthenticate(IEnumerable<Claim> claims)
    {
        var extracted = TokenService.ExtractIdentity(claims);
        if (extracted == null) return false;

        IsAuthenticated = true;
        User = extracted;
        return true;
    }
}