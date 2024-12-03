using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    /// <summary>
    /// Trang đăng nhập, kiểm tra trạng thái khóa tài khoản trước khi hiển thị trang.
    /// </summary>
    /// <returns>
    /// Trả về một đối tượng <see cref="ActionResult">:
    /// - Nếu tài khoản bị khóa, lưu trạng thái khóa vào Session.
    /// - Trả về view "index" cho người dùng.
    /// </returns>
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous, Loginverification]
        public ActionResult Index(string enbLock) //Trang đăng nhập
        {
            if (!string.IsNullOrWhiteSpace(enbLock))
                Session["Locked"] = true;
            return View("index");
        }
    }
}