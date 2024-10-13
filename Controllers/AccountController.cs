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
using System.Text.RegularExpressions;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        private ApplicationUserManager _userManager; //Tạo biến user

        public ApplicationUserManager UserManager //hàm gọi user sau khi login
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); //lấy dữ liệu user sau khi login
            }
            private set
            {
                _userManager = value; //cập nhật dữ liệu user vào biến hiện tại
            }
        }

        [AllowAnonymous, Loginverification]
        public void SignIn() //đăng nhập vào hệ thống
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("SignInCallBack") },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public ActionResult SignOut(string enbLock) //đăng xuất vào hệ thống
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();

            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("index", "home", new { enbLock = enbLock });
        }

        // GET: /Account/SignInCallBack
        public ActionResult SignInCallBack() //tạo hoặc đăng nhập tài khoản hiện có và chuyển hướng sau khi đăng nhập
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
                    return RedirectToAction("SignOut", "Account", new { enbLock = "lock" });
                }
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                string ma = "";
                string hoten = FullName;

                string pattern = string.Format(@"\b{0}\b", " - ");
                int counter = Regex.Matches(FullName, pattern).Count;

                if (counter == 2)
                {
                    FullName = FullName.Replace(" - ", "#");
                    ma = FullName.Split('#')[0].Trim();
                    hoten = FullName.Split('#')[1].Trim() + " - " + FullName.Split('#')[2].Trim();
                }
                else if (counter == 1)
                {
                    FullName = FullName.Replace(" - ", "#");
                    ma = FullName.Split('#')[0];
                    hoten = FullName.Split('#')[1].Trim();
                }

                var aspNetRolesSinhVien = model.AspNetRoles.Where(w => w.ID.Equals("1")).ToList();

                AspNetUsers aspNetUsers = new AspNetUsers();
                aspNetUsers.ID = userId;
                aspNetUsers.Email = EmailUser;
                aspNetUsers.EmailConfirmed = false;
                aspNetUsers.PhoneNumberConfirmed = false;
                aspNetUsers.TwoFactorEnabled = false;
                aspNetUsers.LockoutEnabled = false;
                aspNetUsers.AccessFailedCount = 0;
                aspNetUsers.UserName = ma;
                aspNetUsers.AspNetRoles = aspNetRolesSinhVien;
                model.AspNetUsers.Add(aspNetUsers);
                model.SaveChanges();

                var taikhoanExist = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(userId));
                if (taikhoanExist == null)
                {
                    TaiKhoan newUser = new TaiKhoan();
                    newUser.HoTen = hoten;
                    newUser.Ma = ma;
                    newUser.Email = EmailUser;
                    newUser.ID_AspNetUsers = aspNetUsers.ID;
                    newUser.TrangThai = true;

                    model.TaiKhoan.Add(newUser);
                    model.SaveChanges();
                }
                else
                {
                    taikhoanExist.ID_AspNetUsers = aspNetUsers.ID;
                    model.Entry(taikhoanExist).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }

                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}
