using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class AssignmentController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này trả về view "Assgined" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Assgined".</returns>
        [Authorize, GVRole]
        public ActionResult Assgined()
        {
            return View("Assgined");
        }
    }
}