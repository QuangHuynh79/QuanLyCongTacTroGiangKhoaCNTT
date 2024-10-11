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
        [HttpPost]
        public ActionResult OpenSuggest(int id)
        {
            try
            {
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
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.");

                var lstTask = lhp.CongViec.ToList();
                return PartialView("_TaskList", lstTask);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult AddSuggested(int idLHP, string lydo, string mota, string khoiluong,
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
                for (int i = 0; i < khoiluongs.Count; i++)
                {
                    try
                    {
                        float test = float.Parse(khoiluongs[i].Replace(".", ","));
                    }
                    catch (Exception)
                    {
                        return Content("Sai định dạng-" + i);
                    }
                }

                DeXuatTroGiang dx = new DeXuatTroGiang()
                {
                    ID_LopHocPhan = idLHP,
                    LyDoDeXuat = lydo,
                    TrangThai = false
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