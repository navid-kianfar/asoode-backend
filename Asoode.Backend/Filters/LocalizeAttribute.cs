using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Backend.Filters;

public class LocalizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Path.Value.Contains(".")) return;
        var culture = context.HttpContext.Request.Cookies["culture"];

        if (string.IsNullOrEmpty(culture))
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            culture = configuration["Setting:I18n:Default"];
        }

        var info = CultureInfo.GetCultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = info;
        Thread.CurrentThread.CurrentCulture = info;

        base.OnActionExecuting(context);
    }
}