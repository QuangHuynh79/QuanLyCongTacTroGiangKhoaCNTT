using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace QuanLyCongTacTroGiangKhoaCNTT.Middlewall
{
    public class Loginverification : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult { Data = "SystemLoginAgain", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    filterContext.Result = new RedirectResult("~/studentaffairs/studentaffairsdashboard");
                    return;
                }
            }
        }
    }
}