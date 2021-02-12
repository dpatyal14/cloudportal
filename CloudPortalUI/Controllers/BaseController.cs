using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace CloudPortal.Controllers
{
    public class BaseController : Controller//, ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //check Session here
            if (HttpContext.Session.GetString(Convert.ToString(CloudPortal.Models.SessionExtensions.SessionVals.UserID)) == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                                { "Controller", "Account" },
                                { "Action", "Login" }
                               });
                //  return RedirectToAction("Login", "Account");
            }
        }
    }
}
