using Microsoft.AspNet.Identity.Owin;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class RegisterTeachingAssistantController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: TeachingAssistant
        /// <summary>
        /// Lấy hoặc thiết lập một thể hiện của `ApplicationUserManager`. Nếu chưa được khởi tạo, nó sẽ lấy đối tượng này từ OWIN context.
        /// </summary>
        /// <returns>
        /// Trả về một thể hiện của `ApplicationUserManager`.
        /// </returns>
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
        /// <summary>
        /// Lấy danh sách các form ứng tuyển trợ giảng và hiển thị chúng trên trang đăng ký.
        /// </summary>
        /// <returns>
        /// Trả về một view có tên "Register" cùng với danh sách các form ứng tuyển trợ giảng được sắp xếp theo thứ tự giảm dần của ID.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Register() //Xem danh sách form ứng tuyển
        {
            var lst = model.FormDangKyTroGiang.OrderByDescending(o => o.ID);
            return View("Register", lst);
        }
        /// <summary>
        /// Mở form ứng tuyển trợ giảng cho một ngành học và học kỳ cụ thể, đồng thời kiểm tra tính hợp lệ của các mốc thời gian.
        /// Nếu dữ liệu hợp lệ, lưu thông tin form ứng tuyển và tạo thông báo ứng tuyển.
        /// </summary>
        /// <returns>
        /// Trả về thông báo trạng thái "SUCCESS" nếu việc lưu form ứng tuyển thành công, hoặc một thông báo lỗi nếu có lỗi xảy ra.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddRegister(int hocky, int nganh, DateTime thoigianmo, DateTime thoigiandong) //Mở form ứng tuyển
        {
            try
            {
                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var check = model.FormDangKyTroGiang.FirstOrDefault(f => f.ID_Nganh == nganh && f.ID_HocKy == hocky
                && ((f.ThoiGianMo <= thoigianmo && f.ThoiGianDong >= thoigianmo)
                || (f.ThoiGianMo <= thoigiandong && f.ThoiGianDong >= thoigiandong)));

                if (thoigianmo < currentDate)
                    return Content("NhoHonHienTai");

                if (check != null)
                    return Content("Exist");

                if (thoigianmo >= thoigiandong)
                    return Content("LonHonDangKy");

                var form = new FormDangKyTroGiang();
                form.ID_HocKy = hocky;
                form.ID_Nganh = nganh;
                form.ID_TaiKhoan = Int32.Parse(Session["user-id"].ToString());
                form.ID_TaiKhoanCapNhat = Int32.Parse(Session["user-id"].ToString());
                form.ThoiGianMo = thoigianmo;
                form.ThoiGianDong = thoigiandong;
                form.NgayTao = DateTime.Now;
                form.NgayCapNhat = DateTime.Now;

                model.FormDangKyTroGiang.Add(form);
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();

                var hockyDb = model.HocKy.Find(hocky);
                var thongbao = new ThongBao();

                thongbao.TieuDe = "Ứng tuyển trợ giảng học kỳ " + hockyDb.TenHocKy;
                thongbao.NoiDung = "Ứng tuyển trợ giảng học kỳ " + hockyDb.TenHocKy + " năm học " + hockyDb.NamBatDau + "-" + hockyDb.NamKetThuc
                + ". Thời gian ứng tuyển từ ngày " + thoigianmo.ToString("dd/MM/yyyy") + " đến ngày" + thoigiandong.ToString("dd/MM/yyyy");
                thongbao.ThoiGian = DateTime.Now;
                thongbao.DaDoc = false;
                thongbao.ForRole = "1#4";

                model.ThongBao.Add(thongbao);
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở form cập nhật thông tin của một form đăng ký trợ giảng theo ID. Nếu không tìm thấy form đăng ký, trả về thông báo lỗi.
        /// </summary>
        /// <returns>
        /// Trả về một PartialView "_EditRegister" với dữ liệu của form đăng ký cần cập nhật nếu tìm thấy.
        /// Nếu không tìm thấy, trả về thông báo lỗi với nội dung "Form đăng ký không tồn tại trên hệ thống".
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult OpenEditRegister(int id) //Mở hộp cập nhật form ứng tuyển
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
        /// <summary>
        /// Cập nhật thông tin của một form đăng ký trợ giảng. Nếu có form đăng ký khác trùng với thời gian, trả về thông báo lỗi.
        /// Kiểm tra thời gian đăng ký và đảm bảo không nhỏ hơn thời gian hiện tại.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu cập nhật thành công.
        /// Nếu có lỗi (ví dụ, thời gian trùng với form đăng ký khác hoặc thời gian không hợp lệ), trả về thông báo lỗi cụ thể.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditRegister(int id, int hocky, int nganh, DateTime thoigianmo, DateTime thoigiandong) //Lưu thông tin cập nhật form ứng tuyển
        {
            try
            {
                var check = model.FormDangKyTroGiang.FirstOrDefault(f => f.ID != id && f.ID_Nganh == nganh && f.ID_HocKy == hocky
                && ((f.ThoiGianMo <= thoigianmo && f.ThoiGianDong >= thoigianmo)
                || (f.ThoiGianMo <= thoigiandong && f.ThoiGianDong >= thoigiandong)));
                if (check != null)
                    return Content("Exist");

                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var form = model.FormDangKyTroGiang.Find(id);

                if (thoigianmo != form.ThoiGianMo)
                    if (thoigiandong < currentDate)
                        return Content("NhoHonHienTai");

                if (thoigianmo >= thoigiandong)
                    return Content("LonHonDangKy");

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
        /// <summary>
        /// Xóa form ứng tuyển trợ giảng cùng với các bản ghi ứng tuyển liên quan.
        /// Nếu form ứng tuyển không tồn tại, trả về thông báo "SUCCESS" mà không thực hiện thao tác xóa.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu thao tác xóa thành công.
        /// Nếu có lỗi trong quá trình xóa (ví dụ: không tìm thấy form ứng tuyển), trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult DeleteRegister(int id) //Xóa form ứng tuyển
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
    }
}