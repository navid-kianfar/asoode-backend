using System.Security.Claims;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.User;

namespace Asoode.Shared.Abstraction.Contracts;

public interface IUserIdentityService
{
    public bool IsAuthenticated { get; set; }
    public AuthenticatedUserDto? User { get; set; }
    public bool TryAuthenticate(IEnumerable<Claim> claims);
}