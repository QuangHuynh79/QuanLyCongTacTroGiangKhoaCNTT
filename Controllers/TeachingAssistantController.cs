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
            var lst = model.FormDangKyTroGiang.OrderByDescending(o => o.ID);
            return View("Register", lst);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddRegister(int hocky, int nganh, DateTime thoigianmo, DateTime thoigiandong)
        {
            try
            {
                var check = model.FormDangKyTroGiang.FirstOrDefault(f => f.ID_Nganh == nganh && f.ID_HocKy == hocky
                && ((f.ThoiGianMo <= thoigianmo && f.ThoiGianDong >= thoigianmo)
                || (f.ThoiGianMo <= thoigiandong && f.ThoiGianDong >= thoigiandong)));
                if (check != null)
                    return Content("Exist");

                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                if (thoigianmo <= currentDate)
                    return Content("NhoHonHienTai");

                if (thoigianmo >= thoigiandong)
                    return Content("LonHonDangKy");

                var form = new FormDangKyTroGiang();
                form.ID_HocKy = hocky;
                form.ID_Nganh = nganh;
                form.ID_TaiKhoan = Int32.Parse(Session["user-id"].ToString());
                form.ThoiGianMo = thoigianmo;
                form.ThoiGianDong = thoigiandong;
                form.NgayTao = DateTime.Now;
                form.NgayCapNhat = DateTime.Now;

                model.FormDangKyTroGiang.Add(form);
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult OpenEditRegister(int id)
        {
            try
            {
                var data = model.FormDangKyTroGiang.Find(id);
                if (data == null)
                    return Content("Form đăng ký không tồn tại trên hệ thống.");

                return PartialView("_EditRegister", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditRegister(int id, int hocky, int nganh, DateTime thoigianmo, DateTime thoigiandong)
        {
            try
            {
                var check = model.FormDangKyTroGiang.FirstOrDefault(f => f.ID != id && f.ID_Nganh == nganh && f.ID_HocKy == hocky
                && ((f.ThoiGianMo <= thoigianmo && f.ThoiGianDong >= thoigianmo)
                || (f.ThoiGianMo <= thoigiandong && f.ThoiGianDong >= thoigiandong)));
                if (check != null)
                    return Content("Exist");

                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                if (thoigianmo <= currentDate)
                    return Content("NhoHonHienTai");

                if (thoigianmo >= thoigiandong)
                    return Content("LonHonDangKy");

                var form = model.FormDangKyTroGiang.Find(id);
                form.ID_HocKy = hocky;
                form.ID_Nganh = nganh;
                form.ID_TaiKhoanCapNhat = Int32.Parse(Session["user-id"].ToString());
                form.ThoiGianMo = thoigianmo;
                form.ThoiGianDong = thoigiandong;
                form.NgayCapNhat = DateTime.Now;

                model.Entry(form).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult DeleteRegister(int id)
        {
            try
            {
                var data = model.FormDangKyTroGiang.Find(id);
                if (data == null)
                    return Content("SUCCESS");
                model.UngTuyenTroGiang.RemoveRange(data.UngTuyenTroGiang);
                model.FormDangKyTroGiang.Remove(data);
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
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

        [Authorize, BCNRole]
        public ActionResult OpenSuggest(int id)
        {
            try
            {
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_DetailAdvances", lhp);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        public ActionResult AcceptedAdvances(int id)
        {
            try
            {
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var dx = lhp.DeXuatTroGiang.First();
                bool trangthai = dx.TrangThai;

                if (trangthai)
                    dx.TrangThai = false;
                else
                    dx.TrangThai = true;

                model.Entry(dx).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVandBCNRole]
        public ActionResult Registered()
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 3)
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
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskListTA");
            else
                return PartialView("_TaskListGV");
        }


        [Authorize, TAandGVRole]
        public ActionResult TaskDetail(int id)
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskDetailTA");
            else
                return PartialView("_TaskDetailGV", model.CongViec.Find(id));
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