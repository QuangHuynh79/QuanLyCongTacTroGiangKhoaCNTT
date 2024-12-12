using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class AssignmentController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này trả về view "Assgined" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Assgined".</returns>
        [Authorize, GVRole]
        public ActionResult Assgined()
        {
            return View("Assgined");
        }
        /// <summary>
        /// Lọc danh sách các lớp học phần dựa trên học kỳ, ngành và trạng thái.
        /// </summary>
        /// Trạng thái của phân công:
        /// - "all": Lấy tất cả.
        /// - "true": Lấy các lớp đã được phân công trợ giảng.
        /// - "false": Lấy các lớp chưa được phân công trợ giảng.
        /// </param>
        /// <returns>
        /// Trả về một `PartialView` chứa danh sách các lớp học phần phù hợp hoặc thông báo lỗi nếu có lỗi xảy ra.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult Filter(int hocky, int nganh, string trangthai)
        {
            try
            {
                // Lấy thông tin tài khoản từ session
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

                if (trangthai.Equals("all"))
                {
                    var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.MaCBGD.ToLower().Equals(ma) && w.DeXuatTroGiang.Where(wd => wd.TrangThai == true).Count() > 0).ToList();
                    return PartialView("_FilterAssgined", lstTkb);

                }

                else if (trangthai.Equals("true"))
                {
                    var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.MaCBGD.ToLower().Equals(ma) && w.PhanCongTroGiang.Count() > 0 && w.DeXuatTroGiang.Where(wd => wd.TrangThai == true).Count() > 0).ToList();
                    return PartialView("_FilterAssgined", lstTkb);

                }
                else
                {
                    var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.MaCBGD.ToLower().Equals(ma) && w.PhanCongTroGiang.Count() < 1 && w.DeXuatTroGiang.Where(wd => wd.TrangThai == true).Count() > 0).ToList();
                    return PartialView("_FilterAssgined", lstTkb);

                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Tải danh sách ứng tuyển trợ giảng của một lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` chứa danh sách ứng tuyển trợ giảng của lớp học phần hoặc thông báo lỗi nếu xảy ra lỗi.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult LoadListTA(int lhp)
        {
            try
            {
                var lstTA = model.UngTuyenTroGiang.Where(w => w.ID_LopHocPhan == lhp && w.TrangThai == true).ToList();
                Session["title-update-assign"] = model.LopHocPhan.Find(lhp);
                // Trả về PartialView "_ListTA" chứa danh sách trợ giảng
                return PartialView("_ListTA", lstTA);
            }
            catch (Exception Ex)
            {
                // Nếu xảy ra lỗi, xóa tiêu đề khỏi session
                Session["title-update-assign"] = null;
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Cập nhật thông tin phân công trợ giảng cho một lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về chuỗi "SUCCESS" nếu cập nhật thành công, 
        /// hoặc thông báo lỗi chi tiết trong trường hợp xảy ra lỗi.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult SubmitUpdateAssign(int idtk, int idlhp)
        {
            try
            {
                // Tìm lớp học phần dựa trên ID
                var lhp = model.LopHocPhan.Find(idlhp);
                if (lhp != null)
                {
                    // Lấy thông tin phân công trợ giảng đầu tiên của lớp học phần
                    var phancong = lhp.PhanCongTroGiang.First();
                    phancong.ID_TaiKhoan = idtk;

                    // Đánh dấu đối tượng là đã sửa đổi và lưu thay đổi
                    model.Entry(phancong).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

                    // Cập nhật tài khoản trợ giảng cho tất cả công việc liên quan đến lớp học phần
                    var lstCv = lhp.CongViec.ToList();
                    foreach (var c in lstCv)
                    {
                        c.ID_TaiKhoan = idtk;
                        model.Entry(c).State = System.Data.Entity.EntityState.Modified;
                        model.SaveChanges();
                    }
                }
                else
                {
                    // Trả về thông báo lỗi nếu lớp học phần không tồn tại
                    return Content("Chi tiết lỗi: Lớp học phần không tồn tại!");
                }
                // Trả về thông báo thành công
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                // Trả về thông báo lỗi nếu xảy ra exception
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}