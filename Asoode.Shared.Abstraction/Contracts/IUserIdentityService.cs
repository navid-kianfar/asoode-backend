using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Shared.Abstraction.Contracts;

public interface IUserIdentityService
{
    public bool IsAuthenticated { get; set; }
    public AuthenticatedUserDto? User { get; set; }
    public bool TryAuthenticate(string token);
}