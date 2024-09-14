using Microsoft.AspNet.Identity;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class NotificationsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: Notifications

        [Authorize, BCNRole]
        public ActionResult Index()
        {
            var lstNotify = model.ThongBao.ToList();
            return View("Index", lstNotify);
        }

        [Authorize, BCNRole]
        public ActionResult Delete(int id)
        {
            var tb = model.ThongBao.Find(id);
            if (tb == null)
                return Content("SUCCESS");

            model.ThongBao.Remove(tb);
            model.SaveChanges();

            return Content("SUCCESS");
        }
    }
}