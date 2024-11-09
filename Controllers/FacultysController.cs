using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class FacultysController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: Facultys
        [Authorize, BCNRole]
        public ActionResult Faculty()
        {
            var lstFaculty = model.Khoa.ToList();
            return View("Faculty", lstFaculty);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddFaculty(string tenkhoa)
        {
            try
            {
                var checks = model.Khoa.FirstOrDefault(f => f.TenKhoa.ToLower().Equals(tenkhoa.ToLower()));
                if (checks != null)
                    return Content("Exist");

                var data = new Khoa();
                data.TenKhoa = tenkhoa;

                model.Khoa.Add(data);
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
        public ActionResult OpenEditFaculty(int id)
        {
            try
            {
                var data = model.Khoa.Find(id);
                if (data == null)
                    return Content("Khoa không tồn tại trên hệ thống.");

                return PartialView("_EditFaculty", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditFaculty(int id, string tenkhoa)
        {
            try
            {
                var data = model.Khoa.Find(id);
                data.TenKhoa = tenkhoa;

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
        public ActionResult DeleteFaculty(int id)
        {
            try
            {
                var data = model.Khoa.Find(id);
                if (data == null)
                    return Content("SUCCESS");

                model.Khoa.Remove(data);
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