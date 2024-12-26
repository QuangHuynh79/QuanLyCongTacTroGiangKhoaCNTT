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

                string saveNoti = noti.SetNotification("Duyệt ứng tuyển.", "Bạn đã được duyệt ứng tuyển Lớp HP " + ut.LopHocPhan.MaLHP + ".", "0", ut.ID_TaiKhoan);
               
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
                if (trangthai.ToLower().Equals("all"))
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
                else if (trangthai.ToLower().Equals("true"))
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
                if (trangthai.ToLower().Equals("all"))
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma) && w.TrangThai == true).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else if (trangthai.ToLower().Equals("true"))
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma) && w.TrangThai == true
                    && w.LopHocPhan.PhanCongTroGiang.Where(wp => wp.ID_TaiKhoan == w.ID_TaiKhoan).Count() > 0).ToList();
                    return PartialView("_FilterRegistereds", uts);
                }
                else
                {
                    if (ut == null)
                        return PartialView("_FilterRegistereds", new List<UngTuyenTroGiang>());

                    List<int> idF = new List<int>();
                    foreach (var t in ut)
                        idF.Add(t.ID);
                    var uts = model.UngTuyenTroGiang.Where(w => idF.Contains(w.ID_FormDangKyTroGiang) && w.LopHocPhan.MaCBGD.Equals(tk.Ma) && w.TrangThai == true
                    && w.LopHocPhan.PhanCongTroGiang.Where(wp => wp.ID_TaiKhoan == w.ID_TaiKhoan).Count() < 1).ToList();
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
    }
}