using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Inventory.Helpers;


namespace Inventory.Attributes
{
    public class OnlyAnonymousAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                context.Result = new RedirectResult(Common.BaseURL(context.HttpContext));
            }
            base.OnActionExecuting(context);
        }
    }
}
