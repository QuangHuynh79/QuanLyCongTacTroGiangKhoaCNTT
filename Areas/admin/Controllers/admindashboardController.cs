using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace QuanLyCongTacTroGiangKhoaCNTT.Areas.admin.Controllers
{
    [Authorize]
    public class admindashboardController : Controller
    {
        // GET: admin/admindashboard
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            Session["user-email"] = user;
            
            return View("index");
        }
    }
}