using Microsoft.AspNet.Identity;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
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

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class UsersController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: Users
        [Authorize]
        [BCNRole]
        public ActionResult Index()
        {
            var lstRole = model.Quyen.Where(w => w.ID != 1).ToList();
            return View("Index", lstRole);
        }
        [Authorize]
        public ActionResult LoadContent()
        {
            return PartialView("_Index");
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult AddNew(string ma, string hoten, string email, int chucdanh, string dienthoai, string khoa, string nganh, string gioitinh, bool quoctich)
        {
            try
            {
                var checks = model.TaiKhoan.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower()));
                if (checks != null)
                    return Content("Exist");

                var data = new TaiKhoan();
                data.Email = email;
                data.HoTen = hoten;
                data.ID_Quyen = chucdanh;
                data.TrangThai = true;

                data.Ma = ma;
                data.GioiTinh = gioitinh;
                if (quoctich)
                    data.QuocTich = "Việt Nam";
                else
                    data.QuocTich = "Nước ngoài";
                data.SDT = dienthoai;
                data.Khoa = khoa;
                if (!string.IsNullOrEmpty(nganh))
                    data.ID_Nganh = Int32.Parse(nganh);

                model.TaiKhoan.Add(data);
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult EditState(bool trangthai, int id)
        {
            try
            {
                var data = model.TaiKhoan.Find(id);
                if (data == null)
                    return Content("Người dùng không tồn tại trên hệ thống.");
                data.TrangThai = trangthai;

                model.Entry(data).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }


        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult OpenEdit(int id)
        {
            try
            {
                var data = model.TaiKhoan.Find(id);
                if (data == null)
                    return Content("Người dùng không tồn tại trên hệ thống.");

                return PartialView("_EditUser", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult SubmitEdit(int id, string ma, string hoten, string email, int chucdanh, string dienthoai, string khoa, string nganh, string gioitinh, bool quoctich)
        {
            try
            {
                var checks = model.TaiKhoan.FirstOrDefault(u => (u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower())) && u.ID != id);
                if (checks != null)
                    return Content("Exist");

                var data = model.TaiKhoan.Find(id);
                data.Email = email;
                data.HoTen = hoten;
                data.ID_Quyen = chucdanh;

                data.Ma = ma;
                data.GioiTinh = gioitinh;
                if (quoctich)
                    data.QuocTich = "Việt Nam";
                else
                    data.QuocTich = "Nước ngoài";
                data.SDT = dienthoai;
                data.Khoa = khoa;
                if (!string.IsNullOrEmpty(nganh))
                    data.ID_Nganh = Int32.Parse(nganh);
                model.Entry(data).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var data = model.TaiKhoan.Find(id);
                if (data == null)
                    return Content("SUCCESS");

                model.TaiKhoan.Remove(data);
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult Filter(int id)
        {
            try
            {
                if (id == 0)
                {
                    var data = model.TaiKhoan.ToList().OrderByDescending(o => o.ID);
                    return PartialView("_Filter", data);
                }
                else
                {
                    var data = model.TaiKhoan.Where(w => w.ID_Quyen == id).ToList().OrderByDescending(o => o.ID);
                    return PartialView("_Filter", data);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}