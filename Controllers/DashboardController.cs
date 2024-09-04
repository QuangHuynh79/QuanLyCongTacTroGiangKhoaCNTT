using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            Session["user-email"] = user;

            return View("index");
        }
    }
}