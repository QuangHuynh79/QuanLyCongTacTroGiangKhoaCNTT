using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous, Loginverification]
        public ActionResult Index(string enbLock)
        {
            if (!string.IsNullOrWhiteSpace(enbLock))
                Session["Locked"] = true;
            return View("index");
        }
    }
}