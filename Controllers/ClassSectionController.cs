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

        [Authorize, GVRole]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Authorize, GVRole]
        public ActionResult FilterSection(int hocky, int nganh)
        {
            try
            {
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_Nganh == nganh && w.ID_HocKy == hocky && w.LopHocPhan.MaCBGD.ToLower().Equals(ma)).ToList();

                return PartialView("_FilterSection", lstTkb);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVRole]
        [HttpPost]
        public ActionResult OpenSuggest(int id)
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();

                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

                string name = lhp.TenHP.ToLower();
                var lstDeXuat = model.DeXuatTroGiang.FirstOrDefault(w => w.LopHocPhan.TenHP.ToLower().Equals(name) && w.LopHocPhan.MaCBGD.ToLower().Equals(ma));

                return PartialView("_AddDeXuat", id); //Không Có lớp tương đương để đồng bộ công việc
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVRole]
        [HttpPost]
        public ActionResult SyncTaskList(int id)
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

                string name = lhp.TenHP.ToLower();
                var lstTask = model.DeXuatTroGiang.OrderByDescending(o => o.ID).FirstOrDefault(w => w.LopHocPhan.TenHP.ToLower().Equals(name) && w.LopHocPhan.MaCBGD.ToLower().Equals(ma));

                if (lstTask != null)
                    return PartialView("_LoadSyncTask", lstTask);

                return Content("Chi tiết lỗi: Không có công việc của lớp học phần tương tự để đồng bộ.");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVRole]
        [HttpPost]
        public ActionResult OpenTaskList(int id)
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();

                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var lstTask = lhp.CongViec.ToList();
                if (lhp.DeXuatTroGiang.First().MoCapNhat == true) //Lớp học phần chưa duyệt được phép chỉnh sửa
                    return PartialView("_TaskListViewEdit", lstTask);
                else //Lớp học phần đã duyệt chỉ được phép xem
                    return PartialView("_TaskListOnlyView", lstTask);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVRole]
        [HttpPost]
        public ActionResult EditTaskList(int idLHP, string mota, string khoiluong,
           string thoigian, string noilamviec, string ketqua)
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var motas = mota.Split('~').ToList();
                var khoiluongs = khoiluong.Split('~').ToList();
                var thoigians = thoigian.Split('~').ToList();
                var noilamviecs = noilamviec.Split('~').ToList();
                var ketquas = ketqua.Split('~').ToList();

                //Check int
                for (int i = 1; i <= khoiluongs.Count; i++)
                {
                    try
                    {
                        float test = float.Parse(khoiluongs[i - 1].Replace(".", ","));
                    }
                    catch (Exception)
                    {
                        return Content("Sai định dạng-" + i);
                    }
                }

                var lhp = model.LopHocPhan.Find(idLHP);
                model.CongViec.RemoveRange(lhp.CongViec.ToList());
                model.SaveChanges();

                for (int i = 0; i < motas.Count; i++)
                {
                    CongViec cv = new CongViec()
                    {
                        ID_LopHocPhan = idLHP,
                        MoTa = motas[i],
                        SoGioQuyDoi = float.Parse(khoiluongs[i].Replace(".", ",")),
                        ThoiHanHoanThanh = Convert.ToDateTime(thoigians[i]),
                        NoiLamViec = noilamviecs[i],
                        KetQuaMongDoi = ketquas[i],
                        TrangThai = "canlam",
                    }; model.CongViec.Add(cv);
                }
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();

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


        [Authorize, GVRole]
        [HttpPost]
        public ActionResult AddSuggested(int idLHP, string lydo, bool trangthai, string mota, string khoiluong,
            string thoigian, string noilamviec, string ketqua)
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var motas = mota.Split('~').ToList();
                var khoiluongs = khoiluong.Split('~').ToList();
                var thoigians = thoigian.Split('~').ToList();
                var noilamviecs = noilamviec.Split('~').ToList();
                var ketquas = ketqua.Split('~').ToList();

                //Check int
                for (int i = 1; i <= khoiluongs.Count; i++)
                {
                    try
                    {
                        float test = float.Parse(khoiluongs[i - 1].Replace(".", ","));
                    }
                    catch (Exception)
                    {
                        return Content("Sai định dạng-" + i);
                    }
                }
                var lhp = model.LopHocPhan.Find(idLHP);
                DeXuatTroGiang dx = new DeXuatTroGiang()
                {
                    ID_LopHocPhan = idLHP,
                    LyDoDeXuat = lydo,
                    TrangThai = trangthai,
                    MoCapNhat = !trangthai
                }; model.DeXuatTroGiang.Add(dx);

                for (int i = 0; i < motas.Count; i++)
                {
                    CongViec cv = new CongViec()
                    {
                        ID_LopHocPhan = idLHP,
                        MoTa = motas[i],
                        SoGioQuyDoi = float.Parse(khoiluongs[i].Replace(".", ",")),
                        ThoiHanHoanThanh = Convert.ToDateTime(thoigians[i]),
                        NoiLamViec = noilamviecs[i],
                        KetQuaMongDoi = ketquas[i],
                        TrangThai = "canlam",
                    }; model.CongViec.Add(cv);
                }
                model.SaveChanges();

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

        [Authorize, TARole]
        public ActionResult QuanLyLopHoc()
        {
            return View("QuanLyLopHoc");
        }
    }
}