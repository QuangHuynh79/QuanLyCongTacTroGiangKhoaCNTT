using DocumentFormat.OpenXml.EMMA;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class AdvancesClassectionController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        NotificationsController noti = new NotificationsController();

        /// <summary>
        /// Hiển thị danh sách lớp học phần đã được giảng viên chọn và đề xuất học phần.
        /// Hàm này trả về một view chứa danh sách lớp học phần mà giảng viên đã đề xuất.
        /// </summary>
        /// <returns>
        /// Trả về view "Advances" để hiển thị danh sách lớp học phần đã được giảng viên chọn và đề xuất học phần.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult Advances() //BCN xem danh sách lớp học phần đã được GV chọn đề xuất học phần
        {
            return View("Advances");
        }
        /// <summary>
        /// Lọc danh sách lớp học phần đã được giảng viên chọn và đề xuất học phần theo học kỳ và ngành.
        /// Hàm này sẽ trả về một danh sách các lớp học phần mà giảng viên đã đề xuất trợ giảng, dựa trên học kỳ và ngành học.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_FilterAdvances" chứa danh sách các lớp học phần đã được giảng viên chọn đề xuất học phần.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult FilterAdvances(int hocky, int nganh) //BCN xem danh sách lớp học phần đã được GV chọn đề xuất học phần
        {
            var lstDeXuat = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Count > 0).ToList();
            return PartialView("_FilterAdvances", lstDeXuat);
        }
        /// <summary>
        /// Mở chi tiết mô tả công việc của lớp học phần được giảng viên đề xuất trợ giảng.
        /// Hàm này sẽ lấy thông tin chi tiết về lớp học phần từ cơ sở dữ liệu và trả về partial view để hiển thị thông tin mô tả công việc.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_DetailAdvances" với thông tin chi tiết về lớp học phần được giảng viên đề xuất trợ giảng.
        /// Nếu lớp học phần không tồn tại hoặc đã bị xóa, trả về thông báo lỗi.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult OpenSuggest(int id) //BCN xem chi tiết mô tả CV của LHP được GV đề xuất
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
        /// <summary>
        /// Duyệt bảng mô tả công việc của lớp học phần đã được giảng viên đề xuất trợ giảng.
        /// Hàm này sẽ thay đổi trạng thái của đề xuất trợ giảng (duyệt hoặc hủy duyệt) và thông báo cho giảng viên.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu thao tác thành công.
        /// Nếu lớp học phần không tồn tại hoặc đã bị xóa, trả về thông báo lỗi thích hợp.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AcceptedAdvances(int id) //BCN duyệt bảng mô tả CV của LHP được đề xuất
        {
            try
            {
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var dx = lhp.DeXuatTroGiang.First();
                bool trangthai = dx.TrangThai;

                if (trangthai)
                {
                    dx.TrangThai = false;
                    dx.MoCapNhat = true;
                }
                else
                {
                    dx.TrangThai = true;
                    dx.MoCapNhat = false;
                }
                model.Entry(dx).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                if (!trangthai)
                {
                    var tkNguoiNhan = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lhp.MaCBGD.ToLower()));
                    string saveNoti = noti.SetNotification("Đề xuất trợ giảng.", "Lớp " + lhp.MaLHP + " đã được duyệt đề xuất trợ giảng bởi quản trị viên.", "0", tkNguoiNhan.ID);
                }
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Mở hoặc đóng cập nhật bảng mô tả công việc của lớp học phần đã được giảng viên đề xuất trợ giảng.
        /// Hàm này thay đổi trạng thái của việc cập nhật bảng mô tả công việc và cập nhật trạng thái tương ứng của đề xuất trợ giảng.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu thao tác thành công.
        /// Nếu lớp học phần không tồn tại hoặc đã bị xóa, trả về thông báo lỗi thích hợp.
        /// </returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult EditStateAdvances(int id) //BCN mở-đóng cập nhật bảng mô tả CV của LHP được đề xuất
        {
            try
            {
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var dx = lhp.DeXuatTroGiang.First();
                bool trangthai = dx.MoCapNhat;
                trangthai = !trangthai;

                dx.MoCapNhat = trangthai;
                if (trangthai == true)
                    dx.TrangThai = !trangthai;


                model.Entry(dx).State = System.Data.Entity.EntityState.Modified;
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