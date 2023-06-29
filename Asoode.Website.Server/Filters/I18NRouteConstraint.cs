using Microsoft.AspNetCore.Routing.Constraints;

namespace Asoode.Website.Server.Filters;

public class I18NRouteConstraint : RegexRouteConstraint
{
    public I18NRouteConstraint() : 
        base(@"(fa|en|ar|fr|it|lv|nl|es|ru|ms|da|pt|sv|de|tr|ga|fi|hi)$")
    {
    }
}