using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TeachingAssistantController : Controller
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
                    var thongbao = new ThongBao()
                    {
                        TieuDe = "Đề xuất trợ giảng.",
                        NoiDung = "Lớp " + lhp.MaLHP + "đã được duyệt đề xuất trợ giảng.",
                        ThoiGian = DateTime.Now,
                        DaDoc = false,
                        ForRole = "2",
                        ID_TaiKhoan = lhp.ID_TaiKhoan
                    };
                    model.ThongBao.Add(thongbao);
                    model.SaveChanges();
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
        /// <summary>
        /// Xử lý hiển thị danh sách các sinh viên ứng tuyển trợ giảng đã được BCN duyệt.
        /// - Nếu người dùng có vai trò là BCN (Role 3 hoặc 5), sẽ trả về danh sách của BCN.
        /// - Nếu người dùng là giảng viên, sẽ trả về danh sách sinh viên đã ứng tuyển.
        /// </summary>
        /// <returns>
        /// Trả về PartialView "Registered" nếu người dùng là BCN (Role 3 hoặc 5), 
        /// hoặc trả về PartialView "Registereds" nếu người dùng là giảng viên.
        /// </returns>
        [Authorize, GVandBCNRole]
        public ActionResult Registered() //GV xem danh sách các SV ứng tuyển trợ giảng đã được BCN duyệt, BCN xem ds sinh viên đã ứng tuyển làm trợ giảng
        {
            try
            {
                int role = Int32.Parse(Session["user-role-id"].ToString());
                if (role == 3 || role == 5)
                    return PartialView("Registered"); //BCN
                else
                    return PartialView("Registereds"); //GV
            }
            catch (Exception Ex)
            {
                return Content("Đã xảy ra lỗi! Vui lòng thử lại sau.");
            }
        }
        /// <summary>
        /// Xử lý hiển thị chi tiết thông tin sinh viên đã ứng tuyển trợ giảng, đã được BCN duyệt.
        /// - Nếu thông tin ứng tuyển không tồn tại, trả về thông báo lỗi.
        /// - Nếu tồn tại, trả về chi tiết thông tin ứng tuyển của sinh viên.
        /// </summary>
        /// <returns>
        /// Trả về PartialView "_DetailRegistered" chứa thông tin chi tiết của sinh viên ứng tuyển trợ giảng.
        /// Nếu không tìm thấy thông tin ứng tuyển, trả về thông báo lỗi.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult DetailRegistered(int id) //GV xem chi tiết các SV ứng tuyển trợ giảng đã được BCN duyệt, BCN xem chi tiết sinh viên đã ứng tuyển làm trợ giảng
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_DetailRegistered", ut);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Duyệt ứng tuyển trợ giảng cho sinh viên.
        /// - Cập nhật trạng thái ứng tuyển của sinh viên.
        /// - Thêm thông báo cho người dùng sau khi duyệt.
        /// </summary>
        /// <returns>
        /// Trả về "SUCCESS" nếu duyệt thành công, thông báo lỗi nếu không tìm thấy thông tin ứng tuyển hoặc có lỗi.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult AcceptedRegistered(int id, bool trangthai) //Duyệt ứng tuyển trợ giảng
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                ut.TrangThai = trangthai;

                model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                var thongbao = new ThongBao()
                {
                    TieuDe = "Duyệt ứng tuyển.",
                    NoiDung = "Bạn đã được duyệt ứng tuyển Lớp " + ut.LopHocPhan.MaLHP + ".",
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = "1#4",
                    ID_TaiKhoan = ut.ID_TaiKhoan
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
        //ExportRegistered
        /// <summary>
        /// Lọc danh sách ứng tuyển trợ giảng theo học kỳ, ngành và trạng thái.
        /// - Nếu trạng thái là "all", trả về tất cả các ứng tuyển.
        /// - Nếu trạng thái là "true", chỉ trả về các ứng tuyển có trạng thái duyệt.
        /// - Nếu trạng thái là "false", chỉ trả về các ứng tuyển chưa được duyệt.
        /// </summary>
        /// <returns>
        /// Trả về một phần view với danh sách các ứng tuyển trợ giảng, tùy thuộc vào trạng thái lọc.
        /// Nếu không có ứng tuyển, trả về một danh sách trống.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult FilterRegistered(int hocky, int nganh, string trangthai) //Lọc danh sách ứng tuyển
        {
            try
            {
                if (trangthai.Equals("all"))
                {
                    var ut = model.FormDangKyTroGiang.Where(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh).ToList().OrderByDescending(o => o.ID);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang)).ToList();
                    return PartialView("_FilterRegistered", uts);
                }
                else if (trangthai.Equals("true"))
                {
                    var ut = model.FormDangKyTroGiang.Where(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh).ToList().OrderByDescending(o => o.ID);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.TrangThai == true).ToList();
                    return PartialView("_FilterRegistered", uts);
                }
                else
                {
                    var ut = model.FormDangKyTroGiang.Where(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh).ToList().OrderByDescending(o => o.ID);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.TrangThai == false).ToList();
                    return PartialView("_FilterRegistered", uts);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Xuất danh sách ứng tuyển trợ giảng theo học kỳ, ngành và trạng thái.
        /// - Nếu trạng thái là "all", xuất tất cả các ứng tuyển.
        /// - Nếu trạng thái là "true", xuất các ứng tuyển đã được duyệt.
        /// - Nếu trạng thái là "false", xuất các ứng tuyển chưa được duyệt.
        /// </summary>
        /// <returns>
        /// Trả về một phần view với danh sách ứng tuyển trợ giảng, tùy thuộc vào trạng thái lọc.
        /// Nếu không có ứng tuyển nào thỏa mãn điều kiện, trả về danh sách trống.
        /// Nếu có lỗi trong quá trình xử lý, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, BCNRole]
        public ActionResult ExportRegistered(int hocky, int nganh, string trangthai) //Xuất danh sách ứng tuyển
        {
            try
            {
                if (trangthai.Equals("all"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_ExportRegistered", ut.UngTuyenTroGiang.ToList());
                }
                else if (trangthai.Equals("true"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_ExportRegistered", ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == true).ToList());
                }
                else
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_ExportRegistered", ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == false).ToList());
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Hiển thị chi tiết thông tin ứng tuyển trợ giảng.
        /// Hàm này lấy thông tin ứng tuyển trợ giảng theo ID và hiển thị thông tin chi tiết.
        /// Nếu không tìm thấy thông tin ứng tuyển, trả về thông báo lỗi.
        /// </summary>
        /// <returns>
        /// Trả về một partial view với thông tin chi tiết của ứng tuyển trợ giảng.
        /// Nếu không tìm thấy thông tin ứng tuyển, trả về thông báo lỗi "Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống".
        /// Nếu có lỗi trong quá trình xử lý, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult DetailRegistereds(int id) //TT Chi tiết ứng tuyển
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_DetailRegistereds", ut);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Lọc danh sách ứng tuyển trợ giảng của Giảng viên (GV) theo học kỳ, ngành và trạng thái của ứng tuyển.
        /// Hàm này sẽ lọc các ứng tuyển trợ giảng dựa trên các tiêu chí như học kỳ, ngành và trạng thái đã được duyệt hoặc chưa duyệt.
        /// Nếu không có ứng tuyển nào phù hợp với các tiêu chí lọc, sẽ trả về danh sách rỗng.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_FilterRegistereds" chứa danh sách các ứng tuyển trợ giảng đã được lọc theo các tiêu chí.
        /// Nếu không có ứng tuyển nào phù hợp với các tiêu chí lọc, trả về một danh sách rỗng.
        /// Nếu có lỗi xảy ra trong quá trình xử lý, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult FilterRegistereds(int hocky, int nganh, string trangthai) //Lọc danh sáhc ứng tuyển Role GV
        {
            try
            {
                var tk = Session["Taikhoan"] as TaiKhoan;
                var ut = model.FormDangKyTroGiang.Where(w => w.ID_HocKy == hocky
                && w.ID_Nganh == nganh).ToList().OrderByDescending(o => o.ID);
                if (trangthai.Equals("all"))
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma)).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else if (trangthai.Equals("true"))
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma)
                    && w.DanhGiaPhongVan.Where(wd => wd.KetLuanDat == true).Count() > 0).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma)
                    && (w.DanhGiaPhongVan.Where(wd => wd.KetLuanDat == false).Count() > 0 || w.DanhGiaPhongVan.Count() < 1)).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Xuất danh sách ứng tuyển trợ giảng dựa trên học kỳ, ngành và trạng thái của ứng tuyển.
        /// Hàm này lọc các ứng tuyển trợ giảng theo các tiêu chí học kỳ, ngành và trạng thái ứng tuyển ("all", "true", "false"),
        /// sau đó trả về danh sách ứng tuyển trợ giảng đã được lọc.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_ExportRegistereds" chứa danh sách ứng tuyển trợ giảng đã được lọc theo các tiêu chí.
        /// Nếu không có ứng tuyển nào phù hợp với các tiêu chí lọc, trả về một danh sách rỗng.
        /// Nếu có lỗi xảy ra trong quá trình xử lý, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult ExportRegistereds(int hocky, int nganh, string trangthai)
        {
            try
            {
                if (trangthai.Equals("all"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == true).ToList();
                    return PartialView("_ExportRegistereds", uts);
                }
                else if (trangthai.Equals("true"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(w => w.TrangThai == true && w.DanhGiaPhongVan.Where(ws => ws.KetLuanDat == true).Count() > 0).ToList();
                    return PartialView("_ExportRegistereds", uts);
                }
                else
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_ExportRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(w => w.TrangThai == true
                    && (w.DanhGiaPhongVan.Where(ws => ws.KetLuanDat == false).Count() > 0 || w.DanhGiaPhongVan.Count() < 1)).ToList();
                    return PartialView("_ExportRegistereds", uts);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở form đánh giá phỏng vấn cho ứng tuyển trợ giảng.
        /// Hàm này tìm kiếm thông tin ứng tuyển trợ giảng theo ID, sau đó trả về form để đánh giá phỏng vấn của ứng tuyển đó.
        /// Nếu không tìm thấy ứng tuyển, trả về thông báo lỗi.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_PhongVanTA" chứa thông tin chi tiết về ứng tuyển trợ giảng cho phép đánh giá phỏng vấn.
        /// Nếu không tìm thấy ứng tuyển hoặc có lỗi xảy ra, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult OpenPhongVanRegistereds(int id) //Mở form đánh giá phỏng vấn
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_PhongVanTA", ut);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Xử lý việc gửi kết quả phỏng vấn của ứng viên cho vị trí trợ giảng.
        /// Hàm này nhận các thông tin đánh giá phỏng vấn của ứng viên, cập nhật vào cơ sở dữ liệu và thông báo kết quả.
        /// Nếu kết quả phỏng vấn đạt, thay đổi vai trò người dùng và phân công trợ giảng. Nếu không đạt, gửi thông báo kết quả không đạt.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu quá trình cập nhật và gửi thông báo thành công. 
        /// Nếu có lỗi, trả về thông báo chi tiết lỗi xảy ra.
        /// </returns>
        [Authorize, GVRole] //Lưu thông tin phỏng vấn TA
        public ActionResult SubmitPhongVanRegistereds(int id, string tieuchi, string tongdiem, string nhanxet, bool ketqua, DateTime ngaypv, DateTime ngayduyetpv)
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                var lstTieuChi = tieuchi.Split('#').ToList();
                var lstDiem = tongdiem.Split('#').ToList();
                if (ut.DanhGiaPhongVan.Count > 0)
                {
                    var pv = ut.DanhGiaPhongVan.First();

                    pv.NgoaiHinhTacPhong = lstTieuChi[0];
                    pv.KienThuc = lstTieuChi[1];
                    pv.HieuBietCongViec = lstTieuChi[2];
                    pv.ThaIDoTichCuc = lstTieuChi[3];
                    pv.KyNangTrinhBay = lstTieuChi[4];
                    pv.KyNangGiaoTiep = lstTieuChi[5];
                    pv.GiaiQuyetVanDe = lstTieuChi[6];
                    pv.KhaNangSuPham = lstTieuChi[7];
                    pv.KyNangCNTT = lstTieuChi[8];
                    pv.YeuToKhac = lstTieuChi[9];

                    pv.TongSoDiemChuaDat = int.Parse(lstDiem[0]);
                    pv.TongSoDiemPhanVan = int.Parse(lstDiem[1]);
                    pv.TongSoDiemDat = int.Parse(lstDiem[2]);

                    pv.NhanXet = nhanxet;
                    pv.KetLuanDat = ketqua;
                    pv.NgayPhongVan = ngaypv;
                    pv.NgayDuyet = ngayduyetpv;

                    model.Entry(pv).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }
                else
                {
                    var pv = new DanhGiaPhongVan();

                    pv.ID_UngTuyenTroGiang = id;
                    pv.NgoaiHinhTacPhong = lstTieuChi[0];
                    pv.KienThuc = lstTieuChi[1];
                    pv.HieuBietCongViec = lstTieuChi[2];
                    pv.ThaIDoTichCuc = lstTieuChi[3];
                    pv.KyNangTrinhBay = lstTieuChi[4];
                    pv.KyNangGiaoTiep = lstTieuChi[5];
                    pv.GiaiQuyetVanDe = lstTieuChi[6];
                    pv.KhaNangSuPham = lstTieuChi[7];
                    pv.KyNangCNTT = lstTieuChi[8];
                    pv.YeuToKhac = lstTieuChi[9];

                    pv.TongSoDiemChuaDat = int.Parse(lstDiem[0]);
                    pv.TongSoDiemPhanVan = int.Parse(lstDiem[1]);
                    pv.TongSoDiemDat = int.Parse(lstDiem[2]);

                    pv.NhanXet = nhanxet;
                    pv.KetLuanDat = ketqua;
                    pv.NgayPhongVan = ngaypv;
                    pv.NgayDuyet = ngayduyetpv;

                    model.DanhGiaPhongVan.Add(pv);
                    model.SaveChanges();
                }

                if (ketqua)
                {
                    var aspNetRoles = model.AspNetRoles.Where(w => w.ID.Equals("4")).ToList();

                    string userId = ut.TaiKhoan.AspNetUsers.ID.ToLower();
                    var aspNetUsers = model.AspNetUsers.Find(userId);

                    string idRole = aspNetUsers.AspNetRoles.First().ID;
                    UserManager.RemoveFromRoles(userId, idRole);

                    aspNetUsers.AspNetRoles = aspNetRoles;
                    model.Entry(aspNetUsers).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

                    int idlhp = ut.ID_LopHocPhan;
                    int idtk = ut.ID_TaiKhoan;

                    var pc = new PhanCongTroGiang();
                    pc.ID_LopHocPhan = idlhp;
                    pc.ID_TaiKhoan = idtk;
                    pc.DaNghiViec = false;
                    pc.TrangThai = false;
                    pc.SoGioThucTe = 0;
                    pc.GhiChu = "";

                    model.PhanCongTroGiang.Add(pc);
                    model.SaveChanges();

                    var lstCv = ut.LopHocPhan.CongViec.ToList();
                    foreach (var c in lstCv)
                    {
                        c.ID_TaiKhoan = idtk;
                    }
                    model.Entry(lstCv).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

                    var thongbao = new ThongBao()
                    {
                        TieuDe = "Kết quả phỏng vấn.",
                        NoiDung = ut.TaiKhoan.Ma + " - " + ut.TaiKhoan.HoTen + " đã đạt yêu cầu phỏng vấn vào Lớp " + ut.LopHocPhan.MaLHP + ".",
                        ThoiGian = DateTime.Now,
                        DaDoc = false,
                        ForRole = "1#4",
                        ID_TaiKhoan = ut.ID_TaiKhoan,
                    };
                    model.ThongBao.Add(thongbao);
                    model.SaveChanges();
                }
                else
                {
                    var thongbao = new ThongBao()
                    {
                        TieuDe = "Kết quả phỏng vấn.",
                        NoiDung = ut.TaiKhoan.Ma + " - " + ut.TaiKhoan.HoTen + " không đạt yêu cầu phỏng vấn vào Lớp " + ut.LopHocPhan.MaLHP + ".",
                        ThoiGian = DateTime.Now,
                        DaDoc = false,
                        ForRole = "1#4",
                        ID_TaiKhoan = ut.ID_TaiKhoan,
                    };
                    model.ThongBao.Add(thongbao);
                    model.SaveChanges();
                }

                model = new CongTacTroGiangKhoaCNTTEntities();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở trang đăng ký trợ giảng (TA) cho người dùng có vai trò TA.
        /// Hàm này trả về trang View để ứng viên có thể đăng ký làm trợ giảng.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "Apply" để hiển thị giao diện đăng ký trợ giảng.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult Apply() //Mở trang đăng ký TA role TA
        {
            return View("Apply");
        }
        /// <summary>
        /// Lọc thời khóa biểu dựa trên các tiêu chí và trạng thái đăng ký trợ giảng.
        /// Hàm này sẽ trả về danh sách các lớp học phần phù hợp với các thông tin như học kỳ, ngành, môn học, giảng viên, và trạng thái đăng ký.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với danh sách các lớp học phần đã lọc theo các tiêu chí và trạng thái đăng ký.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost] //Filer LHP, GV, Trạng thái trang đăng ký TA
        public ActionResult FilterChildApply(int hocky, int nganh, string mon, string gv, string trangthai) //Lọc thời khóa biểu để đky TA
        {
            var lstMon = mon.Split('#').ToList();
            var lstGv = gv.Split('#').ToList();

            if (trangthai.Equals("tatca"))
            {
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else if (trangthai.Equals("dadangky"))
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() < 1).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
        }
        /// <summary>
        /// Lọc thời khóa biểu theo học kỳ và ngành học để đăng ký trợ giảng.
        /// Hàm này sẽ trả về danh sách các lớp học phần trong học kỳ và ngành học được chỉ định, 
        /// đồng thời chỉ lấy các lớp học phần đã có đề xuất trợ giảng với trạng thái là true.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với danh sách các lớp học phần đã lọc theo học kỳ và ngành học.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult FilterParentApply(int hocky, int nganh)  //Lọc thời khóa biểu theo học kỳ để đăng ký TA
        {
            var tkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
            return PartialView("_FilterParentApply", tkb);
        }
        /// <summary>
        /// Tải dữ liệu thời khóa biểu (tkb) để đăng ký trợ giảng (TA).
        /// Hàm này trả về một `PartialView` chứa giao diện để đăng ký trợ giảng, bao gồm các lớp học phần có thể đăng ký.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với tên "_Apply", chứa giao diện và dữ liệu thời khóa biểu để người dùng có thể đăng ký trợ giảng.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult LoadContentApply() //Load dữ liệu tkb để đăng ký TA
        {
            return PartialView("_Apply");
        }
        /// <summary>
        /// Lấy kết quả đăng ký trợ giảng của người dùng dựa trên tài khoản hiện tại.
        /// Hàm này truy vấn dữ liệu ứng tuyển trợ giảng của người dùng và trả về danh sách kết quả.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "ResultApply", chứa danh sách các kết quả đăng ký trợ giảng của người dùng, được lấy từ cơ sở dữ liệu.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult ResultApply() //Xem kết quả đăng ký trợ giảng
        {
            var idtk = Int32.Parse(Session["user-id"].ToString());
            var ut = model.UngTuyenTroGiang.Where(w => w.ID_TaiKhoan == idtk).ToList();
            return View("ResultApply", ut);
        }
        /// <summary>
        /// Lọc và lấy kết quả đăng ký trợ giảng của người dùng theo học kỳ và ngành.
        /// Hàm này truy vấn dữ liệu từ bảng đăng ký trợ giảng dựa trên học kỳ và ngành, sau đó lọc ra các ứng tuyển của người dùng theo tài khoản.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với tên "_FilterResultApply", chứa danh sách các kết quả đăng ký trợ giảng của người dùng trong học kỳ và ngành đã chọn.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult FilterResultApply(int hocky, int nganh) //Xem kết quả đăng ký trợ giảng
        {
            var idtk = Int32.Parse(Session["user-id"].ToString());
            var formDky = model.FormDangKyTroGiang.Where(f => f.ID_HocKy == hocky && f.ID_Nganh == nganh).ToList();
            List<int> idForm = new List<int>();
            foreach (var item in formDky)
                idForm.Add(item.ID);

            var ut = model.UngTuyenTroGiang.Where(w => w.ID_TaiKhoan == idtk && idForm.Contains(w.ID_FormDangKyTroGiang)).ToList();
            return PartialView("_FilterResultApply", ut);
        }
        /// <summary>
        /// Mở hộp điền thông tin đăng ký trợ giảng cho lớp học phần được chọn.
        /// Hàm này truy vấn thông tin lớp học phần dựa trên `id` và hiển thị hộp đăng ký trợ giảng cho lớp học phần đó.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "_OpenApply", chứa thông tin lớp học phần để người dùng có thể điền thông tin đăng ký trợ giảng.
        /// Nếu không tìm thấy lớp học phần, trả về nội dung lỗi.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult OpenApply(int id) //Mở hộp điền thông đăng ký trợ giảng cho LHP được chọn
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                return View("_OpenApply", lhp);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở hộp xem chi tiết công việc của lớp học phần cần đăng ký trợ giảng.
        /// Hàm này truy vấn thông tin lớp học phần dựa trên `id` và hiển thị danh sách công việc liên quan đến lớp học phần đó.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "_OpenTaskDetailApply", chứa danh sách công việc của lớp học phần để người dùng có thể xem.
        /// Nếu không tìm thấy lớp học phần, trả về nội dung lỗi.
        /// </returns>
        [HttpPost]
        public ActionResult OpenTaskListDetail(int id) //Mở hộp xem chi tiết công việc LHP cần đăng ký trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                return View("_OpenTaskDetailApply", lhp.CongViec.ToList());
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Lưu thông tin ứng tuyển trợ giảng cho lớp học phần và người ứng tuyển.
        /// Hàm này sẽ xử lý thông tin người ứng tuyển, kiểm tra tính hợp lệ, lưu trữ thông tin ứng tuyển mới hoặc cập nhật thông tin ứng tuyển đã có.
        /// Nếu có tài liệu minh chứng (hình ảnh), hệ thống sẽ lưu trữ và liên kết với thông tin ứng tuyển.
        /// Sau khi hoàn thành, một thông báo sẽ được gửi đến hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một chuỗi thông báo "SUCCESS" nếu ứng tuyển mới được lưu thành công hoặc "SUCCESS2" nếu thông tin ứng tuyển đã được cập nhật.
        /// Nếu có lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult SubmitApply(int idFORM, int idLHP, int idTK, string dienthoai, DateTime ngaysinh,
            string gioitinh, decimal tbctl, decimal drl, decimal dtk, List<HttpPostedFileBase> hamc) //Lưu thông tin ứng tuyển trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(idLHP);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");

                var tk = model.TaiKhoan.Find(idTK);
                var hp = model.LopHocPhan.Find(idLHP);
                var form = model.FormDangKyTroGiang.Find(idFORM);

                if (form == null)
                    return Content("Chi tiết lỗi: " + "Chưa mở đăng ký cho học kỳ này.");
                if (hp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                if (tk == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy dữ liệu người ứng tuyển, vui lòng đăng nhập lại.");

                if (string.IsNullOrEmpty(tk.GioiTinh) || tk.NgaySinh == null || string.IsNullOrEmpty(tk.SDT))
                {
                    tk.SDT = dienthoai;
                    tk.NgaySinh = ngaysinh;
                    tk.GioiTinh = gioitinh;

                    model.Entry(tk).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }
                //Chưa ứng tuyển lưu mới
                if (lhp.UngTuyenTroGiang.FirstOrDefault(f => f.ID_TaiKhoan == idTK) == null)
                {
                    var ut = new UngTuyenTroGiang();
                    ut.ID_FormDangKyTroGiang = idFORM;
                    ut.ID_LopHocPhan = idLHP;
                    ut.ID_TaiKhoan = idTK;
                    ut.MSSV = tk.Ma;
                    ut.Email = tk.Email;
                    ut.HoTen = tk.HoTen;
                    ut.SoDienThoai = dienthoai;
                    ut.NgaySinh = ngaysinh;
                    ut.GioiTinh = gioitinh;
                    ut.DiemRL = drl;
                    ut.DiemTBTL = tbctl;
                    ut.DiemTKMH = dtk;

                    string path = "";
                    string pathDirectory = "";
                    string strListImages = "";
                    int i = 0;
                    if (hamc != null)
                    {
                        foreach (var item in hamc)
                        {
                            if (item != null)
                            {
                                if (item.ContentLength > 0)
                                {
                                    i++;
                                    string fileName = item.FileName;
                                    int indexTypeFile = fileName.LastIndexOf(".");
                                    string fileType = fileName.Substring(indexTypeFile, item.FileName.Length - indexTypeFile);

                                    pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP));
                                    if (!Directory.Exists(pathDirectory))
                                    {
                                        Directory.CreateDirectory(pathDirectory);
                                    }
                                    path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP), tk.Ma + "-MC" + i + fileType);
                                    item.SaveAs(path);
                                    strListImages += "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i + fileType + "#";
                                }
                            }
                        }
                    }

                    ut.HinhAnhMinhChung = strListImages.Substring(0, strListImages.Length - 1); ;
                    ut.TrangThai = false;

                    model.UngTuyenTroGiang.Add(ut);
                    model.SaveChanges();

                    var thongbao = new ThongBao()
                    {
                        TieuDe = "Ứng tuyển trợ giảng.",
                        NoiDung = tk.HoTen + " - " + tk.Ma + " đã ứng tuyển vào Lớp " + lhp.MaLHP + ".",
                        ThoiGian = DateTime.Now,
                        DaDoc = false,
                        ForRole = "3",
                    };
                    model.ThongBao.Add(thongbao);
                    model.SaveChanges();

                    model = new CongTacTroGiangKhoaCNTTEntities();
                    return Content("SUCCESS");
                }
                else //Đã ứng tuyển - cập nhật lại tt ut
                {
                    var ut = lhp.UngTuyenTroGiang.FirstOrDefault(f => f.ID_TaiKhoan == idTK);
                    ut.MSSV = tk.Ma;
                    ut.Email = tk.Email;
                    ut.HoTen = tk.HoTen;
                    ut.SoDienThoai = dienthoai;
                    ut.NgaySinh = ngaysinh;
                    ut.GioiTinh = gioitinh;
                    ut.DiemRL = drl;
                    ut.DiemTBTL = tbctl;
                    ut.DiemTKMH = dtk;

                    string path = "";
                    string pathDirectory = "";
                    string strListImages = "";
                    string lstAnhCu = ut.HinhAnhMinhChung;

                    int i = 0;
                    if (hamc != null)
                    {
                        foreach (var item in hamc)
                        {
                            if (item != null)
                            {
                                if (item.ContentLength > 0)
                                {
                                    string fileName = item.FileName;
                                    int indexTypeFile = fileName.LastIndexOf(".");
                                    string fileType = fileName.Substring(indexTypeFile, item.FileName.Length - indexTypeFile);

                                    i++;
                                    if (!string.IsNullOrEmpty(lstAnhCu))
                                    {
                                        for (int j = 0; j < lstAnhCu.Split('#').Count(); j++)
                                        {
                                            var check = false;
                                            foreach (var items in lstAnhCu.Split('#').ToList())
                                            {
                                                var url = "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i;
                                                var nUrl = items.Substring(0, url.Length);

                                                if (url.Equals(nUrl))
                                                {
                                                    i++;
                                                    check = true;
                                                    break;
                                                }
                                            }
                                            if (check == false)
                                                break;
                                        }
                                    }
                                    pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP));
                                    if (!Directory.Exists(pathDirectory))
                                    {
                                        Directory.CreateDirectory(pathDirectory);
                                    }
                                    path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP), tk.Ma + "-MC" + i + fileType);
                                    item.SaveAs(path);
                                    strListImages += "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i + fileType + "#";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(strListImages))
                        ut.HinhAnhMinhChung = (string.IsNullOrEmpty(lstAnhCu) ? "" : lstAnhCu + "#") + strListImages.Substring(0, strListImages.Length - 1);

                    model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

                    model = new CongTacTroGiangKhoaCNTTEntities();
                    return Content("SUCCESS2");
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Hủy bỏ ứng tuyển trợ giảng cho lớp học phần và tài khoản người ứng tuyển.
        /// Hàm này tìm kiếm thông tin ứng tuyển của người dùng dựa trên ID form đăng ký, ID tài khoản, và ID lớp học phần.
        /// Nếu thông tin ứng tuyển tồn tại, hệ thống sẽ xóa thông tin ứng tuyển và các đánh giá phỏng vấn liên quan.
        /// Sau khi xóa thành công, trả về thông báo "SUCCESS".
        /// </summary>
        /// <returns>
        /// Trả về "SUCCESS" nếu quá trình hủy ứng tuyển thành công. Nếu gặp lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult CancelApply(int idFORM, int idTK, int idLHP) //Mở hộp xem chi tiết công việc LHP cần đăng ký trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var ut = model.UngTuyenTroGiang.FirstOrDefault(f => f.ID_FormDangKyTroGiang == idFORM && f.ID_TaiKhoan == idTK && f.ID_LopHocPhan == idLHP);
                if (ut == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy thông tin ứng tuyển trợ giảng.");

                model.DanhGiaPhongVan.RemoveRange(ut.DanhGiaPhongVan);
                model.UngTuyenTroGiang.Remove(ut);
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
        /// Xóa hình ảnh minh chứng trong thông tin ứng tuyển trợ giảng.
        /// Hàm này tìm kiếm thông tin ứng tuyển của người dùng dựa trên ID form đăng ký, ID tài khoản, và ID lớp học phần.
        /// Sau đó, nếu tìm thấy hình ảnh trong danh sách, hệ thống sẽ xóa hình ảnh đó khỏi thư mục lưu trữ và cập nhật lại thông tin trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>
        /// Trả về "SUCCESS" nếu quá trình xóa hình ảnh thành công. Nếu gặp lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult DeleteImageApply(int idFORM, int idTK, int idLHP, string url) //Xóa hình ảnh minh chứng apply hiện có
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var ut = model.UngTuyenTroGiang.FirstOrDefault(f => f.ID_FormDangKyTroGiang == idFORM && f.ID_TaiKhoan == idTK && f.ID_LopHocPhan == idLHP);
                if (ut == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy thông tin ứng tuyển trợ giảng.");

                if (System.IO.File.Exists(Path.Combine(Server.MapPath(url))))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath(url)));
                }

                var lstImgCu = ut.HinhAnhMinhChung.Split('#').ToList();
                var lstUrlDelete = url.Split('#').ToList();

                string sortImg = "";
                if (lstImgCu.Count > 0)
                {
                    if (lstUrlDelete.Count > 0)
                        foreach (var imgDelete in lstUrlDelete)
                            lstImgCu.Remove(imgDelete);

                    foreach (var f in lstImgCu)
                        sortImg += f.ToString() + "#";

                    if (sortImg.Length > 0)
                        sortImg = sortImg.Substring(0, sortImg.Length - 1);
                }
                ut.HinhAnhMinhChung = sortImg;

                model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
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
        /// Hàm này mở trang quản lý công việc.
        /// </summary>
        /// <returns>
        /// Trả về view "TaskList" để hiển thị giao diện quản lý công việc.
        /// </returns>
        [Authorize, TAandGVRole]
        public ActionResult TaskList() //Quản lý công việc
        {
            return View("TaskList");
        }
        /// <summary>
        /// Hàm này tải dữ liệu cho trang quản lý công việc tùy theo vai trò người dùng.
        /// Nếu người dùng có vai trò là TA, trả về partial view "_TaskListTA", nếu là GV, trả về partial view "_TaskListGV".
        /// </summary>
        /// <returns>
        /// Trả về partial view tương ứng với vai trò của người dùng. 
        /// Nếu người dùng có vai trò TA, trả về "_TaskListTA", nếu người dùng có vai trò GV, trả về "_TaskListGV".
        /// </returns>
        [Authorize, TAandGVRole]
        public ActionResult LoadContentTaskList() //Load dữ liệu quản lý công việc
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskListTA"); //Role TA
            else
                return PartialView("_TaskListGV"); //Role GV
        }
        /// <summary>
        /// Hàm này lọc thông tin lớp học phần và trả về thông tin của lớp học phần được chọn.
        /// Dựa trên ID lớp học phần được truyền vào, hàm tìm kiếm thông tin và trả về partial view với dữ liệu lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về partial view "_FilterTaskList" với thông tin lớp học phần tương ứng.
        /// </returns>
        [Authorize, TAandGVRole]
        [HttpPost] //Lọc danh sách công việc
        public ActionResult FilterTaskList(int lophocphan)
        {
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            var task = model.LopHocPhan.FirstOrDefault(f => f.ID == lophocphan);

            return PartialView("_FilterTaskList", task);
        }
        /// <summary>
        /// Hàm này lọc danh sách lớp học phần trong quản lý công việc theo học kỳ, phân biệt giữa vai trò trợ giảng (TA) và giảng viên (GV).
        /// - Nếu vai trò là TA, lọc lớp học phần mà TA được phân công công việc.
        /// - Nếu vai trò là GV, lọc lớp học phần mà GV giảng dạy và có công việc.
        /// </summary>
        /// <returns>
        /// Trả về partial view "_FilterHocKyTaskListTA" (dành cho TA) hoặc "_FilterHocKyTaskListGV" (dành cho GV) với danh sách lớp học phần theo học kỳ.
        /// </returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult FilterHocKyTaskList(int hocky) //lọc danh sách lớp học phần trong quản lý công việc theo học kỳ
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            if (role == 4)
            {
                int idTk = taikhoan.ID;
                var tasks = model.LopHocPhan.Where(w => w.CongViec.Where(wt => wt.ID_TaiKhoan == idTk).Count() > 0 && w.ID_HocKy == hocky).ToList();
                var task = tasks.Where(w => w.DeXuatTroGiang.First().TrangThai == true).ToList();
                return PartialView("_FilterHocKyTaskListTA", task); //Role TA
            }
            else
            {
                var tasks = model.LopHocPhan.Where(w => w.MaCBGD.ToLower().Equals(ma) && w.CongViec.Count > 0 && w.ID_HocKy == hocky).ToList();
                var task = tasks.Where(w => w.DeXuatTroGiang.First().TrangThai == true).ToList();
                return PartialView("_FilterHocKyTaskListGV", task); //Role GV
            }
        }
        /// <summary>
        /// Hàm này lưu thông tin chỉnh sửa chi tiết công việc, bao gồm trạng thái công việc, ghi chú, và hình ảnh minh chứng.
        /// - Nếu người dùng là giảng viên (role = "gv"), lưu trạng thái hoàn thành hoặc chưa hoàn thành và ghi chú.
        /// - Nếu người dùng là sinh viên (role != "gv"), lưu trạng thái công việc (loại bỏ tiền tố "task" nếu có) và ghi chú.
        /// - Nếu có hình ảnh minh chứng mới, lưu hình ảnh đó vào thư mục tương ứng và xóa hình ảnh cũ nếu yêu cầu.
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu lưu thành công, hoặc thông báo lỗi nếu có lỗi xảy ra.</returns>
        [Authorize, TAandGVRole]
        [HttpPost] //Lưu cập nhật trạng thái công việc
        public ActionResult SubmitEditTaskDetail(int id, string role, string trangthai, string ghichu, HttpPostedFileBase hamc, string deleteImg)
        {
            try
            {
                var tk = Session["Taikhoan"] as TaiKhoan;
                var cv = model.CongViec.Find(id);
                if (cv == null)
                    return Content("Chi tiết lỗi: Không tìm thấy công việc tương ứng.");

                if (role.Equals("gv")) //Là giảng viên thì lưu tình trạng
                {
                    if (trangthai.Equals("hoanthanh") || trangthai.Equals("chuahoanthanh"))
                    {
                        cv.TrangThai = "hoanthanh";
                    }
                    cv.KetQuaCongViec = trangthai;
                    cv.GhiChu = ghichu;
                }
                else // Là sinh viên thì lưu trạng thái
                {
                    if (trangthai.IndexOf("task") == -1) //Dạng kéo thả task sẽ kèm chữ task phía trước để phân biệt
                        cv.GhiChu = ghichu;
                    cv.TrangThai = trangthai.Replace("task", ""); //Xóa chữ task phía trước trạng thái để lưu trạng thái

                    if (!string.IsNullOrEmpty(deleteImg)) //Check xóa img cũ k
                    {
                        bool deleteImgs = Convert.ToBoolean(deleteImg); // Convert string to bool
                        if (deleteImgs)
                        {
                            string url = cv.HinhAnhMinhChung;
                            cv.HinhAnhMinhChung = null;
                            if (System.IO.File.Exists(Path.Combine(Server.MapPath(url))))
                            {
                                System.IO.File.Delete(Path.Combine(Server.MapPath(url)));
                            }
                        }

                        //Lưu hình ảnh minh chứng path Content/HinhAnhMinhChungCongViec/LHP/MSSV-MC1
                        string path = "";
                        string pathDirectory = "";
                        int i = 0;
                        if (hamc != null)
                        {
                            if (hamc.ContentLength > 0)
                            {
                                i++;
                                string fileName = hamc.FileName;
                                int indexTypeFile = fileName.LastIndexOf(".");
                                string fileType = fileName.Substring(indexTypeFile, hamc.FileName.Length - indexTypeFile);

                                pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP));
                                if (!Directory.Exists(pathDirectory))
                                {
                                    Directory.CreateDirectory(pathDirectory);
                                }
                                path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP), tk.Ma + "-MC" + i + fileType);
                                hamc.SaveAs(path);

                                cv.HinhAnhMinhChung = "~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP + "/" + tk.Ma + "-MC" + i + fileType;
                            }
                        }
                    }
                }

                model.Entry(cv).State = System.Data.Entity.EntityState.Modified;
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
        /// Hàm này trả về form chi tiết công việc theo ID công việc.
        /// Tìm công việc từ cơ sở dữ liệu và trả về PartialView với thông tin chi tiết của công việc.
        /// </summary>
        /// <returns>Trả về PartialView "_TaskDetail" với thông tin chi tiết của công việc.</returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult TaskDetail(int id) //Mở form chi tiết công việc
        {
            return PartialView("_TaskDetail", model.CongViec.Find(id));
        }
        /// <summary>
        /// Hàm này trả về view "Assgined" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Assgined".</returns>
        [Authorize, GVRole]
        public ActionResult Assgined()
        {
            return View("Assgined");
        }
    }
}