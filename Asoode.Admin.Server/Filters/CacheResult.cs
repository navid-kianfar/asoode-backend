using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asoode.Admin.Server.Filters;

internal class CacheResult : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is FileResult)
        {
            context.HttpContext.Response.GetTypedHeaders().CacheControl = 
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromHours(12)
                };
        }
    }
}