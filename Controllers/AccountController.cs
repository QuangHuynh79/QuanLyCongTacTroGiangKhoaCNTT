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
using Microsoft.AspNet.Identity.Owin;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous, Loginverification]
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
            ClaimsIdentity identitys = (ClaimsIdentity)User.Identity;
            string FullName = identitys.Claims.SingleOrDefault(s => s.Type.Equals("name")).Value; //get full name
            string userId = EmailUser.ToLower();

            var users = model.AspNetUsers.Find(EmailUser.ToLower());
            if (users != null)
            {
                if (users.LockoutEnabled == true)
                {
                    Session["Locked"] = true;
                    return RedirectToAction("SignOut", "Account");
                }
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                var aspNetRolesSinhVien = model.AspNetRoles.Where(w => w.Id.Equals("1")).ToList();

                AspNetUsers aspNetUsers = new AspNetUsers();
                aspNetUsers.Id = userId;
                aspNetUsers.Email = EmailUser;
                aspNetUsers.EmailConfirmed = false;
                aspNetUsers.PhoneNumberConfirmed = false;
                aspNetUsers.TwoFactorEnabled = false;
                aspNetUsers.LockoutEnabled = false;
                aspNetUsers.AccessFailedCount = 0;
                aspNetUsers.UserName = FullName;
                aspNetUsers.AspNetRoles = aspNetRolesSinhVien;
                model.AspNetUsers.Add(aspNetUsers);
                model.SaveChanges();

                TaiKhoan newUser = new TaiKhoan();
                newUser.HoTen = FullName;
                newUser.Email = EmailUser;
                newUser.ID_AspNetUsers = aspNetUsers.Id;
                newUser.TrangThai = true;

                model.TaiKhoan.Add(newUser);
                model.SaveChanges();

                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}
