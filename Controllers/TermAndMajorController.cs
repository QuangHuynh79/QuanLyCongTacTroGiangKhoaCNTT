using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TermAndMajorController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: TermAndMajor
        [Authorize, BCNRole]
        public ActionResult Semester()
        {
            var lstSemester = model.HocKy.ToList();
            return View("Semester", lstSemester);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddSemester(int tenhocky, int nambatdau, int namketthuc, int tuanbatdau, DateTime ngaybatdau, int tiettoida, int loptoida)
        {
            try
            {
                var checks = model.HocKy.FirstOrDefault(f => f.TenHocKy == tenhocky);
                if (checks != null)
                    return Content("Exist");

                if (nambatdau != ngaybatdau.Year)
                    return Content("INVALIDYEAR");

                var data = new HocKy();
                data.TenHocKy = tenhocky;
                data.NamBatDau = nambatdau;
                data.NamKetThuc = namketthuc;
                data.TuanBatDau = tuanbatdau;
                data.NgayBatDau = ngaybatdau;
                data.TietToiDa = tiettoida;
                data.LopToiDa = loptoida;
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
        public ActionResult EditSemester(int id, int tenhocky, int nambatdau, int namketthuc, int tuanbatdau, DateTime ngaybatdau, int tiettoida, int loptoida)
        {
            try
            {
                if (nambatdau != ngaybatdau.Year)
                    return Content("INVALIDYEAR");

                var data = model.HocKy.Find(id);
                data.TenHocKy = tenhocky;
                data.NamBatDau = nambatdau;
                data.NamKetThuc = namketthuc;
                data.TuanBatDau = tuanbatdau;
                data.NgayBatDau = ngaybatdau;
                data.TietToiDa = tiettoida;
                data.LopToiDa = loptoida;

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

        [Authorize, BCNRole]
        public ActionResult Major()
        {
            var lstMajor = model.Nganh.ToList();
            return View("Major", lstMajor);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddMajor(string manganh, string tennganh, string tenviettat, string ctdt)
        {
            try
            {
                var checks = model.Nganh.FirstOrDefault(f => f.MaNganh.ToLower().Equals(manganh.ToLower()));
                if (checks != null)
                    return Content("Exist");

                var data = new Nganh();
                data.MaNganh = manganh;
                data.TenNganh = tennganh;
                data.TenVietTat = tenviettat;
                data.CTDT = ctdt;

                model.Nganh.Add(data);
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
        public ActionResult OpenEditMajor(int id)
        {
            try
            {
                var data = model.Nganh.Find(id);
                if (data == null)
                    return Content("Ngành không tồn tại trên hệ thống.");

                return PartialView("_EditMajor", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditMajor(int id, string manganh, string tennganh, string tenviettat, string ctdt)
        {
            try
            {
                var data = model.Nganh.Find(id);
                data.MaNganh = manganh;
                data.TenNganh = tennganh;
                data.TenVietTat = tenviettat;
                data.CTDT = ctdt;

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
        public ActionResult DeleteMajor(int id)
        {
            try
            {
                var data = model.Nganh.Find(id);
                if (data == null)
                    return Content("SUCCESS");

                model.Nganh.Remove(data);
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