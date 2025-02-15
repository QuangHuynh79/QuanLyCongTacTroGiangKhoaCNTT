using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
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
    public class InterviewsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        NotificationsController noti = new NotificationsController();

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
        /// Mở form đánh giá phỏng vấn cho ứng tuyển trợ giảng.
        /// Hàm này tìm kiếm thông tin ứng tuyển trợ giảng theo ID, sau đó trả về form để đánh giá phỏng vấn của ứng tuyển đó.
        /// Nếu không tìm thấy ứng tuyển, trả về thông báo lỗi.
        /// </summary>
        /// <returns>
        /// Trả về một partial view "_Interview" chứa thông tin chi tiết về ứng tuyển trợ giảng cho phép đánh giá phỏng vấn.
        /// Nếu không tìm thấy ứng tuyển hoặc có lỗi xảy ra, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, GVRole]
        public ActionResult OpenInterview(int id) //Mở form đánh giá phỏng vấn
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_Interview", ut);
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
        public ActionResult SubmitInterview(int id, string tieuchi, string tongdiem, string nhanxet, bool ketqua, DateTime ngaypv, DateTime ngayduyetpv)
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
                    float tongSoGioQuyDoi = 0;
                    if (ut.LopHocPhan.CongViec.Count() > 0)
                        tongSoGioQuyDoi = float.Parse(ut.LopHocPhan.CongViec.ToList().Sum(s => s.SoGioQuyDoi).ToString());

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
                    pc.SoGioQuyDoi = tongSoGioQuyDoi;

                    model.PhanCongTroGiang.Add(pc);
                    model.SaveChanges();

                    var lstCv = ut.LopHocPhan.CongViec.ToList();
                    foreach (var c in lstCv)
                    {
                        c.ID_TaiKhoan = idtk;
                        model.Entry(c).State = System.Data.Entity.EntityState.Modified;
                        model.SaveChanges();
                    }

                    string saveNoti = noti.SetNotification("Kết quả phỏng vấn.", ut.TaiKhoan.Ma + " - " + ut.TaiKhoan.HoTen + " đã đạt yêu cầu phỏng vấn vào Lớp " + ut.LopHocPhan.MaLHP + ".", "0", ut.ID_TaiKhoan, 11, ut.TaiKhoan.Email, ut.TaiKhoan.HoTen, ut.LopHocPhan.TenHP, "");
                }
                else
                {
                    string saveNoti = noti.SetNotification("Kết quả phỏng vấn.", ut.TaiKhoan.Ma + " - " + ut.TaiKhoan.HoTen + " không đạt yêu cầu phỏng vấn vào Lớp " + ut.LopHocPhan.MaLHP + ".", "0", ut.ID_TaiKhoan, 12, ut.TaiKhoan.Email, ut.TaiKhoan.HoTen, ut.LopHocPhan.TenHP, "");
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
        /// Lấy dữ liệu và xuất file kết quả phỏng vấn TA.
        /// </summary>
        /// <returns>Trả về kết quả phỏng vấn TA thành file excel</returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult ExportData(int id)
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                return PartialView("_Export", ut);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}