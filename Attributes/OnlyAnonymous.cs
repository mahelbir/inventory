using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Inventory.Attributes
{
    public class OnlyAnonymousAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                context.Result = new RedirectResult("/");
            }
            base.OnActionExecuting(context);
        }
    }
}
