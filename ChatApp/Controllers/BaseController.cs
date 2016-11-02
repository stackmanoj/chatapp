using ChatApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Current.UserID == 0)
                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
        }
    }
}
