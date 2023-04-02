using Asoode.Application.Core.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Asoode.Servers.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var op = OperationResult<object>.Validation();
                context.Result = new JsonResult(op);
            }
        }
    }
}