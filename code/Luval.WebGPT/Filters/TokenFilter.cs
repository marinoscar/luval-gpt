using Luval.Framework.Core;
using Luval.Framework.Core.Configuration;
using Luval.WebGPT.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Luval.WebGPT.Filters
{
    public class TokenFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionToken = context.HttpContext.Request.Headers["AppToken"];
            if(!IsValid(sessionToken, context)) context.Result = new UnauthorizedResult();
        }

        private bool IsValid(string sessionToken, ActionExecutingContext context)
        {
            if(string.IsNullOrEmpty(sessionToken)) return false;

            try
            {
                var token = ValidationToken.Decript(sessionToken);
                if(!token.ValidateSquence(DateTime.UtcNow)) return false;
                context.HttpContext.Request.RouteValues["userId"] = token.UserId;
            }
            catch
            {

                return false;
            }
            return true;
        }
    }
}
