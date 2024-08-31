using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Areas.studentaffairs.Controllers
{
    public class studentaffairsdashboardController : Controller
    {
        // GET: studentaffairs/studentaffairsdashboard
        [Authorize]
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            Session["user-email"] = user;

            return View("index");
        }
    }
}