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
        /// <summary>
        /// Lấy danh sách ngành và trả về trang hiển thị danh sách ngành.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult">:
        /// - Danh sách ngành được lấy từ cơ sở dữ liệu và hiển thị trên view "Major".
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Major() //Xem danh sách ngành
        {
            var lstMajor = model.Nganh.ToList();
            return View("Major", lstMajor);
        }
        /// <summary>
        /// Thêm một ngành mới vào hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult">:
        /// - Nếu ngành đã tồn tại, trả về thông báo "Exist".
        /// - Nếu thêm ngành thành công, trả về thông báo "SUCCESS".
        /// - Nếu có lỗi xảy ra trong quá trình thêm, trả về thông báo lỗi.
        /// </returns>
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
        /// <summary>
        /// Mở form cập nhật thông tin ngành.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult">:
        /// - Nếu ngành tồn tại, trả về một PartialView chứa form cập nhật thông tin ngành.
        /// - Nếu ngành không tồn tại, trả về thông báo lỗi "Ngành không tồn tại trên hệ thống".
        /// - Nếu có lỗi xảy ra trong quá trình truy xuất dữ liệu, trả về thông báo lỗi chi tiết.
        /// </returns>
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
        /// <summary>
        /// Cập nhật thông tin ngành trong hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult">:
        /// - Nếu cập nhật thành công, trả về thông báo "SUCCESS".
        /// - Nếu có lỗi xảy ra trong quá trình cập nhật, trả về thông báo lỗi chi tiết.
        /// </returns>
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
        /// <summary>
        /// Xóa ngành khỏi hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult">:
        /// - Nếu ngành được xóa thành công, trả về thông báo "SUCCESS".
        /// - Nếu có lỗi xảy ra trong quá trình xóa, trả về thông báo lỗi chi tiết.
        /// </returns>
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