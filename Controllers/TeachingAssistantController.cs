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
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TeachingAssistantController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        // GET: TeachingAssistant

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

        [Authorize, BCNRole]
        public ActionResult Register() //Xem danh sách form ứng tuyển
        {
            var lst = model.FormDangKyTroGiang.OrderByDescending(o => o.ID);
            return View("Register", lst);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult AddRegister(int hocky, int nganh, DateTime thoigianmo, DateTime thoigiandong) //Mở form ứng tuyển
        {
            try
            {
                var check = model.FormDangKyTroGiang.FirstOrDefault(f => f.ID_Nganh == nganh && f.ID_HocKy == hocky
                && ((f.ThoiGianMo <= thoigianmo && f.ThoiGianDong >= thoigianmo)
                || (f.ThoiGianMo <= thoigiandong && f.ThoiGianDong >= thoigiandong)));
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

                var thongbao = new ThongBao()
                {
                    TieuDe = "Ứng tuyển trợ giảng học kỳ " + form.HocKy.TenHocKy,
                    NoiDung = "Ứng tuyển trợ giảng học kỳ " + form.HocKy.TenHocKy + " năm học " + form.HocKy.NamBatDau + "-" + form.HocKy.NamKetThuc
                    + ". Thời gian ứng tuyển từ ngày " + form.ThoiGianMo.ToString("dd/MM/yyyy") + " đến ngày" + form.ThoiGianDong.Value.ToString("dd/MM/yyyy"),
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = "1#4",
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

        [Authorize, BCNRole]
        public ActionResult Advances() //BCN xem danh sách lớp học phần đã được GV chọn đề xuất học phần
        {
            return View("Advances");
        }

        [Authorize, BCNRole]
        public ActionResult FilterAdvances(int hocky, int nganh) //BCN xem danh sách lớp học phần đã được GV chọn đề xuất học phần
        {
            var lstDeXuat = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Count > 0).ToList();
            return PartialView("_FilterAdvances", lstDeXuat);
        }
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

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

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
                dx.MoCapNhat = !trangthai;

                model.Entry(dx).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVandBCNRole]
        public ActionResult Registered() //GV xem danh sách các SV ứng tuyển trợ giảng đã được BCN duyệt, BCN xem ds sinh viên đã ứng tuyển làm trợ giảng
        {
            try
            {
                int role = Int32.Parse(Session["user-role-id"].ToString());
                if (role == 3)
                    return PartialView("Registered"); //BCN
                else
                    return PartialView("Registereds"); //GV
            }
            catch (Exception Ex)
            {
                return Content("Đã xảy ra lỗi! Vui lòng thử lại sau.");
            }
        }

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

        [Authorize, BCNRole]
        public ActionResult AcceptedRegistered(int id, bool trangthai)
        {
            try
            {
                var ut = model.UngTuyenTroGiang.Find(id);
                if (ut == null)
                    return Content("Chi tiết lỗi: Thông tin ứng tuyển đã bị xóa hoặc không tồn tại trên hệ thống.");

                ut.TrangThai = trangthai;

                model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                if (trangthai)
                {
                    var aspNetRoles = model.AspNetRoles.Where(w => w.ID.Equals("4")).ToList();

                    string userId = ut.TaiKhoan.AspNetUsers.ID.ToLower();
                    var aspNetUsers = model.AspNetUsers.Find(userId);

                    string idRole = aspNetUsers.AspNetRoles.First().ID;
                    UserManager.RemoveFromRoles(userId, idRole);

                    aspNetUsers.AspNetRoles = aspNetRoles;
                    model.Entry(aspNetUsers).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }

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
        [Authorize, BCNRole]
        public ActionResult FilterRegistered(int hocky, int nganh, string trangthai)
        {
            try
            {
                if (trangthai.Equals("all"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_FilterRegistered", ut.UngTuyenTroGiang.ToList());
                }
                else if (trangthai.Equals("true"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_FilterRegistered", ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == true).ToList());
                }
                else
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistered", new List<UngTuyenTroGiang>());
                    return PartialView("_FilterRegistered", ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == false).ToList());
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        public ActionResult ExportRegistered(int hocky, int nganh, string trangthai)
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

        [Authorize, GVRole]
        public ActionResult DetailRegistereds(int id)
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

        [Authorize, GVRole]
        public ActionResult FilterRegistereds(int hocky, int nganh, string trangthai)
        {
            try
            {
                if (trangthai.Equals("all"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(ws => ws.TrangThai == true).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else if (trangthai.Equals("true"))
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(w => w.TrangThai == true && w.DanhGiaPhongVan.Where(ws => ws.KetLuanDat == true).Count() > 0).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else
                {
                    var ut = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky
                    && w.ID_Nganh == nganh);
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    var uts = ut.UngTuyenTroGiang.Where(w => w.TrangThai == true
                    && (w.DanhGiaPhongVan.Where(ws => ws.KetLuanDat == false).Count() > 0 || w.DanhGiaPhongVan.Count() < 1)).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

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

        [Authorize, GVRole]
        public ActionResult OpenPhongVanRegistereds(int id)
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

        [Authorize, GVRole]
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
                    int idlhp = ut.ID_LopHocPhan;
                    int idtk = ut.ID_TaiKhoan;

                    var pc = new PhanCongTroGiang();
                    pc.ID_LopHocPhan = idlhp;
                    pc.ID_TaiKhoan = idtk;
                    pc.DaNghiViec = false;
                    pc.TrangThai = false;
                    pc.GhiChu = "";

                    model.PhanCongTroGiang.Add(pc);
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

        [Authorize, SVandTARole]
        public ActionResult Apply()
        {
            return View("Apply");
        }

        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult FilterChildApply(int hocky, int nganh, string mon, string gv, string trangthai) //Lọc thời khóa biểu để đky TA
        {
            var lstMon = mon.Split('#').ToList();
            var lstGv = gv.Split('#').ToList();

            if (trangthai.Equals("tatca"))
            {
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH) && lstGv.Contains(w.LopHocPhan.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else if (trangthai.Equals("dadangky"))
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.LopHocPhan.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH) && lstGv.Contains(w.LopHocPhan.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.LopHocPhan.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() < 1).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH) && lstGv.Contains(w.LopHocPhan.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
        }

        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult FilterParentApply(int hocky, int nganh)  //Lọc thời khóa biểu theo học kỳ để đăng ký TA
        {
            var tkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
            return PartialView("_FilterParentApply", tkb);
        }

        [Authorize, SVandTARole]
        public ActionResult LoadContentApply() //Load dữ liệu tkb để đăng ký TA
        {
            return PartialView("_Apply");
        }

        [Authorize, SVandTARole]
        public ActionResult ResultApply() //Xem kết quả đăng ký trợ giảng
        {
            return View("ResultApply");
        }

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
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

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

        [Authorize, TAandGVRole]
        public ActionResult TaskList()
        {
            return View("TaskList");
        }

        [Authorize, TAandGVRole]
        public ActionResult LoadContentTaskList()
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskListTA");
            else
                return PartialView("_TaskListGV");
        }

        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult FilterTaskList(int lophocphan)
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            var task = model.LopHocPhan.FirstOrDefault(f => f.ID == lophocphan);

            if (role == 4)
                return PartialView("_FilterTaskListTA", task);
            else
                return PartialView("_FilterTaskListGV", task);
        }

        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult FilterHocKyTaskList(int hocky)
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            var tasks = model.LopHocPhan.Where(w => w.MaCBGD.ToLower().Equals(ma) && w.CongViec.Count > 0 && w.ID_HocKy == hocky).ToList();
            var task = tasks.Where(w => w.DeXuatTroGiang.First().TrangThai == true).ToList();

            if (role == 4)
                return PartialView("_FilterHocKyTaskListTA", task);
            else
                return PartialView("_FilterHocKyTaskListGV", task);
        }

        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult SubmitEditTaskDetail(int id, string role, string trangthai, string ghichu)
        {
            try
            {
                var cv = model.CongViec.Find(id);
                if (cv == null)
                    return Content("Chi tiết lỗi: Không tìm thấy công việc tương ứng.");

                if (role.Equals("gv"))
                {
                    if (trangthai.Equals("hoanthanh") || trangthai.Equals("chuahoanthanh"))
                    {
                        cv.TrangThai = "hoanthanh";
                    }
                    cv.KetQuaCongViec = trangthai;
                    cv.GhiChu = ghichu;
                }
                else
                {
                    cv.TrangThai = trangthai;
                    cv.GhiChu = ghichu;
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

        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult TaskDetail(int id)
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskDetailTA");
            else
                return PartialView("_TaskDetailGV", model.CongViec.Find(id));
        }

        [Authorize, TARole]
        public ActionResult Evaluation()
        {
            return PartialView("Evaluation");
        }

        [Authorize, GVRole]
        public ActionResult Assgined()
        {
            return View("Assgined");
        }
    }
}