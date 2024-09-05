using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class UserController : Controller
    {
        TrogiangvluEntities model = new TrogiangvluEntities();

        // GET: User
        public ActionResult Index()
        {
            //var lstUser = model.User.ToList();
            return View("Index");
        }
    }
}