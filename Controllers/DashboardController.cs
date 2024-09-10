using Microsoft.AspNet.Identity;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class DashboardController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: Dashboard

        [Authorize]
        [AllRole]
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            Session["user-email"] = user;
            var role = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(user.ToLower()));
            Session["user-role-name"] = role.Quyen.name;
            Session["user-role-id"] = role.ID_Quyen;

            return View("index");
        }
    }
}