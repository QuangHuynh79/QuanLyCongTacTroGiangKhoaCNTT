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
        /// <summary>
        /// Lấy danh sách các khoa từ cơ sở dữ liệu và hiển thị trên trang.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"/> chứa danh sách các khoa.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Faculty() //Xem danh sách khoa
        {
            var lstFaculty = model.Khoa.ToList();
            return View("Faculty", lstFaculty);
        }
        /// <summary>
        /// Thêm một khoa mới vào hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một chuỗi thông báo:
        /// - "Exist" nếu khoa đã tồn tại.
        /// - "SUCCESS" nếu thêm khoa thành công.
        /// - Chi tiết lỗi nếu có sự cố trong quá trình thực thi.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddFaculty(string tenkhoa) //Thêm khoa
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
        /// <summary>
        /// Mở form cập nhật thông tin khoa theo ID.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"/>:
        /// - Nếu khoa tồn tại, trả về form cập nhật (Partial View) với thông tin khoa.
        /// - Nếu khoa không tồn tại, trả về thông báo lỗi "Khoa không tồn tại trên hệ thống."
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult OpenEditFaculty(int id) //Mở form cập nhật khoa
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
        /// <summary>
        /// Cập nhật thông tin khoa với tên mới.
        /// </summary>
        /// <returns>
        /// Trả về một chuỗi thông báo:
        /// - "SUCCESS" nếu cập nhật thành công.
        /// - Chi tiết lỗi nếu có sự cố trong quá trình cập nhật.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditFaculty(int id, string tenkhoa) //Lưu thông tin cập nhật khoa
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
        public ActionResult DeleteFaculty(int id) //Xóa khoa ĐÃ BỎ FUNCTION NÀY
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