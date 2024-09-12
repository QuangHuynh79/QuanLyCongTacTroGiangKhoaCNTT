using Microsoft.AspNet.Identity;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TeachingAssistantController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: TeachingAssistant

        [Authorize]
        [BCNRole]
        public ActionResult Register()
        {
            return View("Register");
        }

        [Authorize]
        [SVRole]
        public ActionResult Apply()
        {
            return View("Apply");
        }

        [Authorize]
        [SVRole]
        public ActionResult LoadContentApply()
        {
            return PartialView("_Apply");
        }

        [Authorize]
        [TARole]
        public ActionResult TaskList()
        {
            return View("TaskList");
        }

        [Authorize]
        [TARole]
        public ActionResult LoadContentTaskList()
        {
            return PartialView("_TaskList");
        }
    }
}