using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class MajorsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: Majors
        [Authorize, BCNRole]
        public ActionResult Major() //Xem danh sách ngành
        {
            var lstMajor = model.Nganh.ToList();
            return View("Major", lstMajor);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddMajor(string manganh, string tennganh, int khoa) //Thêm ngành
        {
            try
            {
                var checks = model.Nganh.FirstOrDefault(f => f.MaNganh.ToLower().Equals(manganh.ToLower()));
                if (checks != null)
                    return Content("Exist");

                var data = new Nganh();
                data.MaNganh = manganh;
                data.TenNganh = tennganh;
                data.ID_Khoa = khoa;

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
        public ActionResult OpenEditMajor(int id) //Mở form cập nhật ngành
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
        public ActionResult EditMajor(int id, string manganh, string tennganh, int khoa) //Lưu thông tin cập nhật ngành
        {
            try
            {
                var data = model.Nganh.Find(id);
                data.MaNganh = manganh;
                data.TenNganh = tennganh;
                data.ID_Khoa = khoa;

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
        public ActionResult DeleteMajor(int id) //Xóa ngành
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