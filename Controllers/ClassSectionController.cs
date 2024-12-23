using Microsoft.AspNet.Identity;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class ClassSectionController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: ClassSection 
        /// <summary>
        /// Tải danh sách lớp học phần và hiển thị giao diện danh sách lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về một <see cref="ActionResult"/> hiển thị giao diện "Index".
        /// </returns>
        [Authorize, GVRole]
        public ActionResult Index() //Load danh sách lớp học phần
        {
            return View("Index");
        }

        /// <summary>
        /// Hiển thị trang quản lý lớp học.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"/> chứa trang "QuanLyLopHoc".
        /// </returns>
        [Authorize, TARole]
        public ActionResult QuanLyLopHoc()
        {
            return View("QuanLyLopHoc");
        }

        /// <summary>
        /// Lọc danh sách lớp học phần dựa trên học kỳ và ngành học, trả về danh sách lớp phù hợp.
        /// </summary>
        /// <returns>
        /// Trả về một <see cref="PartialViewResult"/> hiển thị danh sách lớp học phần được lọc.
        /// Nếu có lỗi xảy ra, trả về nội dung chi tiết lỗi dưới dạng văn bản.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult FilterSection(int hocky, int nganh) //Lọc lớp học phần
        {
            try
            {
                // Lấy thông tin tài khoản từ Session
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();
                // Lọc danh sách lớp học phần dựa trên điều kiện
                var lstTkb = model.LopHocPhan.Where(w => w.ID_Nganh == nganh && w.ID_HocKy == hocky && w.MaCBGD.ToLower().Equals(ma)).ToList();
                // Trả về PartialView "_FilterSection" cùng với danh sách lớp học phần
                return PartialView("_FilterSection", lstTkb);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Lọc danh sách lớp học phần dựa trên học kỳ và ngành học, trả về danh sách lớp phù hợp.
        /// </summary>
        /// <returns>
        /// Trả về một <see cref="PartialViewResult"/> hiển thị danh sách lớp học phần được lọc.
        /// Nếu có lỗi xảy ra, trả về nội dung chi tiết lỗi dưới dạng văn bản.
        /// </returns>
        [Authorize, TARole]
        public ActionResult FilterQuanLyLopHoc(int hocky, int nganh) //Lọc lớp học phần
        {
            try
            {
                // Lấy thông tin tài khoản từ Session
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                // Lọc danh sách lớp học phần dựa trên điều kiện
                var lstTkb = model.LopHocPhan.Where(w => w.ID_Nganh == nganh && w.ID_HocKy == hocky && w.PhanCongTroGiang.Where(wp => wp.ID_TaiKhoan == taikhoan.ID).Count() > 0).ToList();
                // Trả về PartialView "_FilterSection" cùng với danh sách lớp học phần
                return PartialView("_FilterQuanLyLopHoc", lstTkb);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Mở form đề xuất trợ giảng và mô tả công việc dựa trên ID lớp học phần.
        /// </summary>
        /// <param name="id">ID của lớp học phần cần mở form đề xuất.</param>
        /// <returns>
        /// Trả về một <see cref="PartialViewResult"/> để hiển thị form đề xuất trợ giảng.
        /// Nếu lớp học phần không tồn tại hoặc xảy ra lỗi, trả về thông báo lỗi dưới dạng văn bản.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult OpenSuggest(int id) //Mở form đề xuất trợ giảng và mô tả công việc
        {
            try
            {
                // Khởi tạo lại context cho cơ sở dữ liệu
                model = new CongTacTroGiangKhoaCNTTEntities();
                // Tìm lớp học phần dựa trên ID
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    // Nếu không tìm thấy lớp học phần, trả về nội dung thông báo lỗi
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");
                // Trả về PartialView "_AddDeXuat" cùng với ID của lớp học phần
                return PartialView("_AddDeXuat", id);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Đồng bộ danh sách công việc từ lớp học phần tương đương để sử dụng khi đề xuất lớp học phần.
        /// </summary>
        /// <param name="id">ID của lớp học phần cần đồng bộ danh sách công việc.</param>
        /// <returns>
        /// Trả về một <see cref="PartialViewResult"/> để hiển thị danh sách công việc đồng bộ.
        /// Nếu không tìm thấy lớp học phần hoặc danh sách công việc tương đương, trả về thông báo lỗi dưới dạng văn bản.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult SyncTaskList(int id) //Coppy danh sách công việc học phần tương đương khi đề xuất lhp
        {
            try
            {
                // Khởi tạo lại context cho cơ sở dữ liệu
                model = new CongTacTroGiangKhoaCNTTEntities();
                // Tìm lớp học phần dựa trên ID
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");
                // Lấy thông tin tài khoản từ Session
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();
                // Tìm danh sách công việc của lớp học phần tương đương
                string name = lhp.TenHP.ToLower();
                var lstTask = model.DeXuatTroGiang.OrderByDescending(o => o.ID).FirstOrDefault(w => w.LopHocPhan.TenHP.ToLower().Equals(name) && w.LopHocPhan.MaCBGD.ToLower().Equals(ma));

                if (lstTask != null)
                    // Nếu tìm thấy danh sách công việc, trả về PartialView để hiển thị
                    return PartialView("_LoadSyncTask", lstTask);

                return Content("Chi tiết lỗi: Không có công việc của lớp học phần tương tự để đồng bộ.");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở danh sách công việc của lớp học phần, kiểm tra trạng thái để cho phép chỉnh sửa hoặc chỉ xem.
        /// </summary>
        /// <param name="id">ID của lớp học phần cần mở danh sách công việc.</param>
        /// <returns>
        /// Trả về một <see cref="PartialViewResult"/> để hiển thị danh sách công việc.
        /// - Nếu lớp học phần chưa duyệt, trả về view cho phép chỉnh sửa danh sách công việc.
        /// - Nếu lớp học phần đã duyệt, chỉ trả về view để xem danh sách công việc mà không cho phép chỉnh sửa.
        /// Nếu lớp học phần không tồn tại hoặc có lỗi, trả về thông báo lỗi dưới dạng văn bản.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult OpenTaskList(int id) //Mở danh sách công việc
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                // Tìm lớp học phần dựa trên ID
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");
                // Lấy danh sách công việc của lớp học phần
                var lstTask = lhp.CongViec.ToList();
                if (lhp.DeXuatTroGiang.First().MoCapNhat == true || lhp.DeXuatTroGiang.First().TrangThai == false) //Lớp học phần chưa duyệt được phép chỉnh sửa
                    return PartialView("_TaskListViewEdit", lstTask); //Được phép chỉnh sửa
                else //Lớp học phần đã duyệt chỉ được phép xem
                    return PartialView("_TaskListOnlyView", lstTask); //Không dược chỉnh sửa
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Cập nhật mô tả chi tiết công việc của lớp học phần và lưu các thông tin liên quan.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ContentResult"/>:
        /// - "SUCCESS" nếu việc cập nhật thành công.
        /// - Nếu có lỗi trong quá trình thực hiện, trả về thông báo lỗi với chi tiết.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult EditTaskList(int idLHP, string mota, string khoiluong,
           string thoigian, string noilamviec, string ketqua, bool camket) // Lưu cập nhật mô tả công việc lhp
        {
            try
            {
                // Tách các chuỗi mô tả công việc, khối lượng, thời gian, nơi làm việc, kết quả ra thành danh sách
                model = new CongTacTroGiangKhoaCNTTEntities();
                var motas = mota.Split('~').ToList();
                var khoiluongs = khoiluong.Split('~').ToList();
                var thoigians = thoigian.Split('~').ToList();
                var noilamviecs = noilamviec.Split('~').ToList();
                var ketquas = ketqua.Split('~').ToList();

                // Kiểm tra định dạng số cho khối lượng công việc
                for (int i = 1; i <= khoiluongs.Count; i++)
                {
                    try
                    {
                        float test = float.Parse(khoiluongs[i - 1]);
                    }
                    catch (Exception)
                    {
                        return Content("Sai định dạng-" + i);
                    }
                }
                // Tìm lớp học phần theo ID và xóa tất cả công việc hiện tại
                var lhp = model.LopHocPhan.Find(idLHP);
                model.CongViec.RemoveRange(lhp.CongViec.ToList());
                model.SaveChanges();
                // Thêm các công việc mới vào cơ sở dữ liệu
                for (int i = 0; i < motas.Count; i++)
                {
                    CongViec cv = new CongViec()
                    {
                        ID_LopHocPhan = idLHP,
                        MoTa = motas[i],
                        SoGioQuyDoi = float.Parse(khoiluongs[i]),
                        ThoiHanHoanThanh = Convert.ToDateTime(thoigians[i]),
                        NoiLamViec = noilamviecs[i],
                        KetQuaMongDoi = ketquas[i],
                        TrangThai = "canlam",
                    }; model.CongViec.Add(cv);
                }
                model.SaveChanges();
                // Cập nhật trạng thái cam kết của đề xuất trợ giảng
                var dexuat = lhp.DeXuatTroGiang.First();
                dexuat.TrangThai = camket;
                if (camket)
                    dexuat.MoCapNhat = !camket;

                model.Entry(dexuat).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();
                // Tạo thông báo về việc cập nhật công việc
                var tk = Session["TaiKhoan"] as TaiKhoan;
                var thongbao = new ThongBao()
                {
                    TieuDe = "Chi tiết công việc được cập nhật.",
                    NoiDung = "Lớp " + lhp.MaLHP + " đã được cập nhật mô tả chi tiết công việc trợ giảng bởi " + tk.Ma + ".",
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = "3",
                };
                model.ThongBao.Add(thongbao);
                model.SaveChanges();

                model = new CongTacTroGiangKhoaCNTTEntities();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Lưu thông tin đề xuất trợ giảng cho lớp học phần và các công việc liên quan.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ContentResult"/>:
        /// - "SUCCESS" nếu việc lưu thông tin đề xuất và các công việc thành công.
        /// - Nếu có lỗi trong quá trình thực hiện, trả về thông báo lỗi với chi tiết.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult AddSuggested(int idLHP, string lydo, string giothucte, bool trangthai, string mota, string khoiluong,
            string thoigian, string noilamviec, string ketqua) // Lưu thông tin đề xuất lhp
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var motas = mota.Split('~').ToList();
                var khoiluongs = khoiluong.Split('~').ToList();
                var thoigians = thoigian.Split('~').ToList();
                var noilamviecs = noilamviec.Split('~').ToList();
                var ketquas = ketqua.Split('~').ToList();

                // Kiểm tra định dạng số cho khối lượng công việc
                for (int i = 1; i <= khoiluongs.Count; i++)
                {
                    try
                    {
                        float test = float.Parse(khoiluongs[i - 1]);
                    }
                    catch (Exception)
                    {
                        return Content("Sai định dạng-" + i);
                    }
                }
                // Tìm lớp học phần theo ID và tạo đề xuất trợ giảng
                var lhp = model.LopHocPhan.Find(idLHP);
                DeXuatTroGiang dx = new DeXuatTroGiang()
                {
                    ID_LopHocPhan = idLHP,
                    LyDoDeXuat = lydo,
                    TrangThai = trangthai,
                    MoCapNhat = !trangthai,
                }; model.DeXuatTroGiang.Add(dx);
                // Thêm các công việc mới vào cơ sở dữ liệu
                for (int i = 0; i < motas.Count; i++)
                {
                    CongViec cv = new CongViec()
                    {
                        ID_LopHocPhan = idLHP,
                        MoTa = motas[i],
                        SoGioQuyDoi = float.Parse(khoiluongs[i]),
                        ThoiHanHoanThanh = Convert.ToDateTime(thoigians[i]),
                        NoiLamViec = noilamviecs[i],
                        KetQuaMongDoi = ketquas[i],
                        TrangThai = "canlam",
                    }; model.CongViec.Add(cv);
                }
                model.SaveChanges();
                // Tạo thông báo về việc đề xuất trợ giảng
                var thongbao = new ThongBao()
                {
                    TieuDe = "Đề xuất trợ giảng.",
                    NoiDung = "Lớp " + lhp.MaLHP + " đã được đề xuất trợ giảng bởi " + lhp.TenCBGD + ".",
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = "3",
                };
                model.ThongBao.Add(thongbao);
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