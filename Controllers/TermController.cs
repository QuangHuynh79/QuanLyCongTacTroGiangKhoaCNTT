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
        /// <summary>
        /// Hàm này lấy danh sách tất cả các học kỳ từ cơ sở dữ liệu và trả về view "Semester" cùng với danh sách đó.
        /// </summary>
        /// <returns>Trả về View "Semester" với danh sách học kỳ.</returns>
        [Authorize, BCNRole]
        public ActionResult Semester() //Xem danh sách học kỳ
        {
            var lstSemester = model.HocKy.ToList();
            return View("Semester", lstSemester);
        }
        /// <summary>
        /// Hàm này thêm một học kỳ mới vào cơ sở dữ liệu. Trước khi thêm, nó kiểm tra xem học kỳ đã tồn tại hay chưa, và nếu hợp lệ, nó sẽ thêm học kỳ mới.
        /// </summary>
        /// <returns>Trả về thông báo trạng thái, bao gồm "SUCCESS" nếu thêm thành công, "Exist" nếu học kỳ đã tồn tại, hoặc "INVALIDYEAR" nếu năm không hợp lệ.</returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddSemester(string tenhocky, int nambatdau, int namketthuc, DateTime ngaybatdau) //Thêm học kỳ
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
        /// <summary>
        /// Hàm này dùng để cập nhật trạng thái của một học kỳ trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>Trả về thông báo trạng thái:
        /// - "SUCCESS" nếu cập nhật thành công,
        /// - "Học kỳ không tồn tại trên hệ thống." nếu không tìm thấy học kỳ với ID tương ứng,
        /// - Thông báo lỗi chi tiết nếu có lỗi trong quá trình xử lý.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult editStateSemester(bool trangthai, int id) //Cập nhật trạng thái học kỳ
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
        /// <summary>
        /// Hàm này dùng để mở form cập nhật thông tin học kỳ.
        /// </summary>
        /// <returns>Trả về một partial view "_EditSemester" với dữ liệu học kỳ cần chỉnh sửa nếu tìm thấy, 
        /// hoặc thông báo lỗi nếu học kỳ không tồn tại trên hệ thống.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult OpenEditSemester(int id) //Mở form cập nhật học kỳ
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
        /// <summary>
        /// Hàm này dùng để cập nhật thông tin học kỳ trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu cập nhật thành công, 
        /// hoặc thông báo lỗi chi tiết nếu có lỗi xảy ra trong quá trình xử lý.</returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditSemester(int id, int nambatdau, int namketthuc, DateTime ngaybatdau) //Lưu thông tin cập nhật hk
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
        public ActionResult DeleteSemester(int id) //Xóa hk ĐÃ LOẠI BỎ FUNCITON NÀY
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