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

        [Authorize, GVandBCNRole]
        public ActionResult ListTA()
        {
            return View("ListTA");
        }

        [Authorize, BCNRole]
        public ActionResult Advances()
        {
            return View("Advances");
        }
        [Authorize, GVandBCNRole]
        public ActionResult Registered()
        {
            var role = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(User.Identity.Name.ToLower()));
            if (role.ID_Quyen == 4)
                return PartialView("Registered"); //BCN
            else
                return PartialView("Registereds"); //GV
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
        [Authorize, TAandGVRole]
        public ActionResult TaskList()
        {
            return View("TaskList");
        }

        [Authorize, TAandGVRole]
        public ActionResult LoadContentTaskList()
        {
            var role = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(User.Identity.Name.ToLower()));
            if (role.ID_Quyen == 5)
                return PartialView("_TaskListTA");
            else
                return PartialView("_TaskListGV");
        }

        [Authorize, TARole]
        public ActionResult Evaluation()
        {
            return PartialView("Evaluation");
        }

        [Authorize, GVRole]
        public ActionResult Assgined()
        {
            return View("Assgined");
        }
    }
}