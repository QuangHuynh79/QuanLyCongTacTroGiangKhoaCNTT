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
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [Loginverification]
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("SignInCallBack") },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        [HttpPost]
        public ActionResult SignOut()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("index", "home");
        } 

        // GET: /Account/SignInCallBack
        public ActionResult SignInCallBack()
        {
            return RedirectToAction("index", "studentaffairsdashboard", new { area = "studentaffairs" });
        }
    }
}
