using Microsoft.AspNet.Identity;
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

                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                if (thoigianmo <= currentDate)
                    return Content("NhoHonHienTai");

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

                if (form.ThoiGianMo != thoigianmo)
                    if (thoigianmo <= currentDate)
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

        [Authorize, GVandBCNRole]
        public ActionResult ListTA() //GV và BCN Xem danh sách trợ giảng đã được chọn làm TA
        {
            return View("ListTA");
        }

        [Authorize, BCNRole]
        public ActionResult Advances() //BCN xem danh sách lớp học phần đã được GV chọn đề xuất học phần
        {
            return View("Advances");
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
                    dx.TrangThai = false;
                else
                    dx.TrangThai = true;

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