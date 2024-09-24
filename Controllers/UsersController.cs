using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class UsersController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        [Authorize, BCNRole]
        public ActionResult Index()
        {
            var lstRole = model.AspNetRoles.ToList();
            return View("Index", lstRole);
        }

        [Authorize, BCNRole]
        public ActionResult LoadContent()
        {
            return PartialView("_Index");
        }

        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult AddNew(string ma, string hoten, string email, string chucdanh, string dienthoai, string khoa, string nganh, string gioitinh, bool quoctich, DateTime? ngaysinh)
        {
            try
            {
                var checks = model.TaiKhoan.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower()));
                if (checks != null)
                    return Content("Exist");

                string userId = email.ToLower();
                var aspNetRoles = model.AspNetRoles.Where(w => w.Id.Equals(chucdanh)).ToList();

                AspNetUsers aspNetUsers = new AspNetUsers();
                aspNetUsers.Id = userId;
                aspNetUsers.Email = email;
                aspNetUsers.EmailConfirmed = false;
                aspNetUsers.PhoneNumberConfirmed = false;
                aspNetUsers.TwoFactorEnabled = false;
                aspNetUsers.LockoutEnabled = false;
                aspNetUsers.AccessFailedCount = 0;
                aspNetUsers.UserName = hoten;
                aspNetUsers.AspNetRoles = aspNetRoles;
                model.AspNetUsers.Add(aspNetUsers);
                model.SaveChanges();

                var data = new TaiKhoan();
                data.Email = email;
                data.HoTen = hoten;
                data.ID_AspNetUsers = aspNetUsers.Id;
                data.TrangThai = true;
                data.NgaySinh = ngaysinh;
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

                var aspUser = data.AspNetUsers;
                aspUser.LockoutEnabled = true;

                model.Entry(aspUser).State = System.Data.Entity.EntityState.Modified;
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

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult SubmitEdit(int id, string ma, string hoten, string email, string chucdanh, string dienthoai, string khoa, string nganh, string gioitinh, bool quoctich, DateTime? ngaysinh)
        {
            try
            {
                var checks = model.TaiKhoan.FirstOrDefault(u => (u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower())) && u.ID != id);
                if (checks != null)
                    return Content("Exist");

                var aspNetRoles = model.AspNetRoles.Where(w => w.Id.Equals(chucdanh)).ToList();

                string userId = email.ToLower();
                var aspNetUsers = model.AspNetUsers.Find(userId);

                string idRole = aspNetUsers.AspNetRoles.First().Id;
                UserManager.RemoveFromRoles(userId, idRole);

                aspNetUsers.AspNetRoles = aspNetRoles;
                aspNetUsers.Email = email;
                aspNetUsers.UserName = hoten;
                model.Entry(aspNetUsers).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                var data = model.TaiKhoan.Find(id);
                data.Email = email;
                data.HoTen = hoten;
                data.NgaySinh = ngaysinh;
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

                var aspUser = data.AspNetUsers;

                string userId = data.Email.ToLower();
                var aspNetUsers = model.AspNetUsers.Find(userId);

                string idRole = aspNetUsers.AspNetRoles.First().Id;
                UserManager.RemoveFromRoles(userId, idRole);

                model.TaiKhoan.Remove(data);
                model.AspNetUsers.Remove(aspUser);
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
                    var data = model.TaiKhoan.Where(w => w.AspNetUsers.AspNetRoles.First().Id.Equals(id)).ToList().OrderByDescending(o => o.ID);
                    return PartialView("_Filter", data);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, AllRole]
        [HttpPost]
        public ActionResult UpdateInfo(string ma, string hoten, string dienthoai, string khoa, string nganh, string gioitinh, bool quoctich, DateTime? ngaysinh)
        {
            try
            {
                int id = Int32.Parse(Session["user-id"].ToString());

                var checks = model.TaiKhoan.FirstOrDefault(u => u.Ma.ToLower().Equals(ma.ToLower()) && u.ID != id);
                if (checks != null)
                    return Content("Exist");

                var data = model.TaiKhoan.Find(id);
                data.HoTen = hoten;
                data.NgaySinh = ngaysinh;
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

    }
}