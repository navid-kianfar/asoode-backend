using System.Security.Claims;
using Asoode.Application.Core.Enums.Membership;

namespace Asoode.Application.Core.Contracts;

public interface IAccountService
{
    ClaimsPrincipal ExtractToken(string token, UserType role);
}