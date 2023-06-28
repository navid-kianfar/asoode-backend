using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;

namespace Asoode.Admin.Server.Services;

internal class UserIdentityService : IUserIdentityService
{
    public bool IsAuthenticated { get; set; }
    public AuthenticatedUserDto? User { get; set; }
    public bool TryAuthenticate(string token)
    {
        throw new NotImplementedException();
    }
}