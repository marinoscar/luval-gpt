using Microsoft.AspNetCore.Mvc.Filters;
using Twilio.Security;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Luval.GPT.WebApi.Config;
using Luval.Framework.Core.Configuration;

namespace Luval.GPT.WebApi.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateTwilioRequestAttribute : ActionFilterAttribute
    {
        private readonly RequestValidator _requestValidator;
        
        public ValidateTwilioRequestAttribute()
        {
            var token = ConfigManager.Get("TwilioSecret");
            _requestValidator = new RequestValidator(token);
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var context = actionContext.HttpContext;
            if (!IsValidRequest(context.Request))
            {
                actionContext.Result = new ForbidResult();
            }

            base.OnActionExecuting(actionContext);
        }

        private bool IsValidRequest(HttpRequest request)
        {
            var signature = request.Headers["X-Twilio-Signature"];
            if(string.IsNullOrEmpty(signature)) 
                return false;

            var requestUrl = request.GetDisplayUrl() + "/" + request.QueryString.ToString();
            var val = _requestValidator.Validate(requestUrl, request.Form.ToNVC(), signature);
            return true;
        }
    }
}
