using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Membership.Authentication;

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

    public bool IsAuthenticated { get; private set; }
    public Guid UserId { get; private set; }
    public string Username { get; private set; }
    public UserType UserType { get; private set; }
}