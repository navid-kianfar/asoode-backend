using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class TokenClaimsViewModel
{
    public TokenClaimsViewModel(string username, Guid userId, UserType type = UserType.User)
    {
        IsAuthenticated = true;
        UserId = userId;
        Username = username;
        UserType = type;
    }

    public TokenClaimsViewModel()
    {
        IsAuthenticated = false;
        UserId = Guid.Empty;
        Username = string.Empty;
        UserType = UserType.Anonymous;
    }

    public bool IsAuthenticated { get; }
    public Guid UserId { get; }
    public string Username { get; }
    public UserType UserType { get; }
}