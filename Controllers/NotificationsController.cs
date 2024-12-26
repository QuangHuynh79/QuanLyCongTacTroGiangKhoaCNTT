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

        /// <summary>
        /// Lấy danh sách tất cả các thông báo và hiển thị chúng trên trang chủ.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"> chứa danh sách thông báo dưới dạng một danh sách các đối tượng ThongBao.
        /// Dữ liệu sẽ được hiển thị trên view "Index".
        /// </returns>
        public ActionResult LoadNotification() //Load thông báo
        {
            var lstNotify = model.ThongBao.ToList();
            return View("Index", lstNotify);
        }

        /// <summary>
        /// Lưu thông tin thông báo xuống db.
        /// </summary>
        /// <returns>
        /// Trả về dữ liệu thông báo được lưu xuống database
        /// </returns>
        public string SetNotification(string title, string content, string forRole, int? idTk) //Lưu thông báo
        {
            try
            {
                var thongbao = new ThongBao()
                {
                    TieuDe = title,
                    NoiDung = content,
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = forRole,
                    ID_TaiKhoan = idTk
                };
                model.ThongBao.Add(thongbao);
                model.SaveChanges();
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                return "Chi tiết lỗi: " + Ex.Message;
            }
        }

        /// <summary>
        /// Xóa một thông báo theo ID.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ContentResult"> với thông báo "SUCCESS" nếu xóa thành công.
        /// Nếu không tìm thấy thông báo, trả về "SUCCESS" để báo rằng không có thông báo nào được xóa.
        /// </returns>
        public ActionResult Delete(int id) //Xóa thông báo
        {
            var tb = model.ThongBao.Find(id);
            if (tb == null)
                return Content("SUCCESS");

            model.ThongBao.Remove(tb);
            model.SaveChanges();

            return Content("SUCCESS");
        }


        /// <summary>
        /// Tìm kiếm thông báo theo nội dung.
        /// </summary>
        /// <returns>
        /// Trả về danh sách thông báo khớp với nội dung tìm kiếm theo nội dung thông báo.
        /// </returns>
        public ActionResult Search(string search) //Tìm kiếm thông báo
        {
            if (string.IsNullOrEmpty(search))
            {
                var tb = Session["list-noti-default"] as List<ThongBao>;
                return PartialView("_Search", tb);
            }
            else
            {
                var tbDefault = Session["list-noti-default"] as List<ThongBao>;
                var tb = tbDefault.Where(w => w.NoiDung.ToLower().Contains(search.ToLower()) || w.TieuDe.ToLower().Contains(search.ToLower())).ToList();
                return PartialView("_Search", tb);
            }
        }
    }
}