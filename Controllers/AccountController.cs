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
        /// <summary>
        /// Truy cập đến đối tượng <see cref="ApplicationUserManager"/> để lấy thông tin người dùng sau khi đăng nhập.
        /// Nếu đối tượng chưa được khởi tạo, nó sẽ được khởi tạo từ <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>Đối tượng <see cref="ApplicationUserManager"/> chứa thông tin quản lý người dùng hiện tại.</returns>
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
        /// <summary>
        /// Thực hiện đăng nhập vào hệ thống bằng cách kích hoạt quá trình xác thực thông qua OpenID Connect.
        /// </summary>
        [AllowAnonymous, Loginverification]
        public void SignIn() //đăng nhập vào hệ thống
        {
            try
            {
                // Gửi yêu cầu xác thực với OpenID Connect và chuyển hướng đến URL callback sau khi hoàn thành
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("SignInCallBack") },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            catch (Exception)
            {
                Url.Action("signout");
            }
        }
        /// <summary>
        /// Thực hiện đăng xuất khỏi hệ thống, xóa dữ liệu phiên và bộ nhớ đệm, 
        /// đồng thời đăng xuất khỏi hệ thống xác thực hiện tại.
        /// </summary>
        /// <param name="enbLock">Chuỗi dữ liệu được truyền để sử dụng trong chuyển hướng sau khi đăng xuất.</param>
        /// <returns>Kết quả chuyển hướng đến trang "index" của controller "home" với tham số <paramref name="enbLock"/>.</returns>
        public ActionResult SignOut(string enbLock) //đăng xuất vào hệ thống
        {
            // Vô hiệu hóa bộ nhớ đệm HTTP
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            // Xóa và hủy phiên làm việc hiện tại
            Session.Clear();
            Session.Abandon();
            // Đăng xuất khỏi hệ thống xác thực
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            // Chuyển hướng về trang "index" của controller "home"
            return RedirectToAction("index", "home", new { enbLock = enbLock });
        }

        // GET: /Account/SignInCallBack
        /// <summary>
        /// Xử lý callback sau khi đăng nhập thành công. 
        /// Tạo tài khoản mới nếu chưa tồn tại hoặc đăng nhập vào tài khoản hiện có.
        /// Kiểm tra trạng thái tài khoản và chuyển hướng phù hợp.
        /// </summary>
        /// <returns>
        /// Trả về <see cref="ActionResult"/> thực hiện các thao tác sau:
        /// - Chuyển hướng đến trang SignOut nếu tài khoản bị khóa.
        /// - Chuyển hướng đến trang Dashboard nếu đăng nhập thành công.
        /// - Tạo tài khoản mới và sau đó chuyển hướng đến Dashboard nếu tài khoản chưa tồn tại.
        /// </returns>
        public ActionResult SignInCallBack() //tạo hoặc đăng nhập tài khoản hiện có và chuyển hướng sau khi đăng nhập
        {
            // Lấy email từ thông tin người dùng đã đăng nhập
            var EmailUser = User.Identity.GetUserName();
            ClaimsIdentity identitys = (ClaimsIdentity)User.Identity;
            string FullName = identitys.Claims.SingleOrDefault(s => s.Type.Equals("name")).Value; //get full name
            string userId = EmailUser.ToLower();

            // Kiểm tra tài khoản trong cơ sở dữ liệu
            var users = model.AspNetUsers.Find(EmailUser.ToLower()); //Tài khoản đã tồn tại - load thông tin tài khoản đó
            if (users != null)
            {
                // Nếu tài khoản bị khóa, chuyển hướng đến SignOut
                if (users.LockoutEnabled == true)
                {
                    return RedirectToAction("SignOut", "Account", new { enbLock = "lock" });
                }
                // Nếu tài khoản tồn tại và không bị khóa, chuyển hướng đến Dashboard
                return RedirectToAction("Index", "DashBoard");
            }
            else //Tài khoản chưa tồn tại, tạo tài khoản mới
            {

                //Lấy thông tin họ tên email của tài khoản VLU
                string ma = null;
                string hoten = FullName;
                // Tách mã và họ tên từ chuỗi FullName nếu định dạng phù hợp
                string pattern = string.Format(@"\b{0}\b", " - ");
                int counter = Regex.Matches(FullName, pattern).Count;

                if (counter == 2)
                {
                    FullName = FullName.Replace(" - ", "#");
                    ma = FullName.Split('#')[0].Trim();
                    hoten = FullName.Split('#')[1].Trim() + " - " + FullName.Split('#')[2].Trim();
                }
                // Gán vai trò mặc định cho người dùng
                var aspNetRolesSinhVien = model.AspNetRoles.Where(w => w.ID.Equals("1")).ToList();

                //Tạo aspnet user
                AspNetUsers aspNetUsers = new AspNetUsers();
                aspNetUsers.ID = userId;
                aspNetUsers.Email = EmailUser;
                aspNetUsers.EmailConfirmed = false;
                aspNetUsers.PhoneNumberConfirmed = false;
                aspNetUsers.TwoFactorEnabled = false;
                aspNetUsers.LockoutEnabled = false;
                aspNetUsers.AccessFailedCount = 0;
                aspNetUsers.UserName = ma != null ? ma : hoten;
                aspNetUsers.AspNetRoles = aspNetRolesSinhVien;
                model.AspNetUsers.Add(aspNetUsers);
                model.SaveChanges();

                model = new CongTacTroGiangKhoaCNTTEntities();
                //Kiểm tra tài khoản đã được thêm trc chưa, nếu chưa thì tạo mới
                var taikhoanExist = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(userId));
                if (taikhoanExist == null)
                {
                    // Nếu chưa có tài khoản trong bảng TaiKhoan, tạo mới
                    TaiKhoan newUser = new TaiKhoan();
                    newUser.HoTen = hoten;
                    if (ma != null)
                        newUser.Ma = ma;

                    newUser.Email = EmailUser;
                    newUser.ID_AspNetUsers = userId;
                    newUser.TrangThai = true;

                    model.TaiKhoan.Add(newUser);
                    model.SaveChanges();
                }
                else
                {
                    // Nếu đã có tài khoản, cập nhật thông tin liên kết
                    taikhoanExist.ID_AspNetUsers = userId;
                    model.Entry(taikhoanExist).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }
                // Chuyển hướng đến Dashboard
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}
