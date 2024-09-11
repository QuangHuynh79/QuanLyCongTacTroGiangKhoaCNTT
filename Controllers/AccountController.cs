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
using QuanLyCongTacTroGiangKhoaCNTT.Models;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

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

        public ActionResult SignOut()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();

            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("index", "home");
        }

        // GET: /Account/SignInCallBack
        public ActionResult SignInCallBack()
        {
            var EmailUser = User.Identity.GetUserName();
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string FullName = identity.Claims.ToList()[7].Value;

            var users = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(EmailUser.ToLower()));
            if (users != null)
            {
                if (users.TrangThai == false)
                {
                    Session["Locked"] = true;
                    return RedirectToAction("SignOut", "Account");
                }
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                TaiKhoan newUser = new TaiKhoan();
                newUser.HoTen = FullName;
                newUser.Email = EmailUser;
                newUser.ID_Quyen = 1;
                newUser.TrangThai = true;

                model.TaiKhoan.Add(newUser);
                model.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}
