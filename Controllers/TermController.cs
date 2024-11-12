using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TermController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: Term
        [Authorize, BCNRole]
        public ActionResult Semester()
        {
            var lstSemester = model.HocKy.ToList();
            return View("Semester", lstSemester);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddSemester(string tenhocky, int nambatdau, int namketthuc, DateTime ngaybatdau)
        {
            try
            {
                var checks = model.HocKy.FirstOrDefault(f => f.TenHocKy.Equals(tenhocky));
                if (checks != null)
                    return Content("Exist");

                if (ngaybatdau.Year < nambatdau || ngaybatdau.Year > namketthuc)
                    return Content("INVALIDYEAR");

                var data = new HocKy();
                data.TenHocKy = tenhocky;
                data.NamBatDau = nambatdau;
                data.NamKetThuc = namketthuc;
                data.NgayBatDau = ngaybatdau;
                data.TrangThai = false;

                model.HocKy.Add(data);
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
        public ActionResult editStateSemester(bool trangthai, int id)
        {
            try
            {
                var data = model.HocKy.Find(id);
                if (data == null)
                    return Content("Học kỳ không tồn tại trên hệ thống.");
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

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult OpenEditSemester(int id)
        {
            try
            {
                var data = model.HocKy.Find(id);
                if (data == null)
                    return Content("Học kỳ không tồn tại trên hệ thống.");

                return PartialView("_EditSemester", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditSemester(int id, int nambatdau, int namketthuc, DateTime ngaybatdau)
        {
            try
            {
                if (ngaybatdau.Year < nambatdau || ngaybatdau.Year > namketthuc)
                    return Content("INVALIDYEAR");

                var data = model.HocKy.Find(id);
                data.NamBatDau = nambatdau;
                data.NamKetThuc = namketthuc;
                data.NgayBatDau = ngaybatdau;

                model.Entry(data).State = System.Data.Entity.EntityState.Modified;
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
        public ActionResult DeleteSemester(int id)
        {
            try
            {
                var data = model.HocKy.Find(id);
                if (data == null)
                    return Content("SUCCESS");

                model.HocKy.Remove(data);
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