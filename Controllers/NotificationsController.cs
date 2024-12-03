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
        /// <summary>
        /// Lấy danh sách tất cả các thông báo và hiển thị chúng trên trang chủ.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"> chứa danh sách thông báo dưới dạng một danh sách các đối tượng ThongBao.
        /// Dữ liệu sẽ được hiển thị trên view "Index".
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Index() //Load thông báo
        {
            var lstNotify = model.ThongBao.ToList();
            return View("Index", lstNotify);
        }
        /// <summary>
        /// Xóa một thông báo theo ID.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ContentResult"> với thông báo "SUCCESS" nếu xóa thành công.
        /// Nếu không tìm thấy thông báo, trả về "SUCCESS" để báo rằng không có thông báo nào được xóa.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Delete(int id) //Xóa thông báo
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