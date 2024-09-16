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

        [Authorize, BCNRole]
        public ActionResult Register()
        {
            return View("Register");
        }

        [Authorize, BCNRole]
        public ActionResult ListTA()
        {
            return View("ListTA");
        }

        [Authorize, BCNRole]
        public ActionResult Advances()
        {
            return View("Advances");
        }
        [Authorize, BCNRole]
        public ActionResult Registered()
        {
            return View("Registered");
        }

        [Authorize, SVandTARole]
        public ActionResult Apply()
        {
            return View("Apply");
        }

        [Authorize, SVandTARole]
        public ActionResult LoadContentApply()
        {
            return PartialView("_Apply");
        }

        [Authorize, SVandTARole]
        public ActionResult ResultApply()
        {
            return View("ResultApply");
        }
        [Authorize, TARole]
        public ActionResult TaskList()
        {
            return View("TaskList");
        }

        [Authorize, TARole]
        public ActionResult LoadContentTaskList()
        {
            return PartialView("_TaskList");
        }

        [Authorize, TARole]
        public ActionResult Evaluation()
        {
            return PartialView("Evaluation");
        }
    }
}