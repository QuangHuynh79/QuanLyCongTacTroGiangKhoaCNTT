using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class UsersController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        private ApplicationUserManager _userManager;
        /// <summary>
        /// Thuộc tính để lấy hoặc thiết lập đối tượng ApplicationUserManager.
        /// Nếu _userManager chưa được khởi tạo, nó sẽ lấy từ OWIN context.
        /// </summary>
        /// <returns>Đối tượng ApplicationUserManager</returns>
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

        // GET: Users
        /// <summary>
        /// Hiển thị danh sách các vai trò người dùng.
        /// </summary>
        /// <returns>Trả về view "Index" với danh sách vai trò người dùng</returns>
        [Authorize, BCNRole]
        public ActionResult Index() //Xem danh sách người dùng
        {
            var lstRole = model.AspNetRoles.ToList();
            return View("Index", lstRole);
        }
        /// <summary>
        /// Tải dữ liệu và trả về một partial view có tên "_Index".
        /// </summary>
        /// <returns>Trả về partial view "_Index"</returns>
        [Authorize, BCNRole]
        public ActionResult LoadContent() //Load dữ liệu danh sách người dùng
        {
            return PartialView("_Index");
        }

        //[Authorize]
        //[BCNRole]
        //[HttpPost]
        //public ActionResult AddNew(string ma, string hoten, string email, string chucdanh, string dienthoai, string nganh, string gioitinh, bool quoctich, DateTime? ngaysinh)
        //{
        //    try
        //    {
        //        var checks = model.TaiKhoan.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower()));
        //        if (checks != null)
        //            return Content("Exist");

        //        string userId = email.ToLower();
        //        var aspNetRoles = model.AspNetRoles.Where(w => w.ID.Equals(chucdanh)).ToList();

        //        AspNetUsers aspNetUsers = new AspNetUsers();
        //        aspNetUsers.ID = userId;
        //        aspNetUsers.Email = email;
        //        aspNetUsers.EmailConfirmed = false;
        //        aspNetUsers.PhoneNumberConfirmed = false;
        //        aspNetUsers.TwoFactorEnabled = false;
        //        aspNetUsers.LockoutEnabled = false;
        //        aspNetUsers.AccessFailedCount = 0;
        //        aspNetUsers.UserName = hoten;
        //        aspNetUsers.AspNetRoles = aspNetRoles;
        //        model.AspNetUsers.Add(aspNetUsers);
        //        model.SaveChanges();

        //        var data = new TaiKhoan();
        //        data.Email = email;
        //        data.HoTen = hoten;
        //        data.ID_AspNetUsers = aspNetUsers.ID;
        //        data.TrangThai = true;
        //        data.NgaySinh = ngaysinh;
        //        data.Ma = ma;
        //        data.GioiTinh = gioitinh;
        //        if (quoctich)
        //            data.QuocTich = "Việt Nam";
        //        else
        //            data.QuocTich = "Nước ngoài";
        //        data.SDT = dienthoai;
        //        if (!string.IsNullOrEmpty(nganh))
        //            data.ID_Nganh = Int32.Parse(nganh);

        //        model.TaiKhoan.Add(data);
        //        model.SaveChanges();

        //        return Content("SUCCESS");
        //    }
        //    catch (Exception Ex)
        //    {
        //        return Content("Chi tiết lỗi: " + Ex.Message);
        //    }
        //}
        /// <summary>
        /// Cập nhật trạng thái khóa tài khoản người dùng (mở khóa hoặc khóa tài khoản).
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu thành công, hoặc thông báo lỗi nếu có lỗi xảy ra</returns>
        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult EditState(bool trangthai, int id) //Cập nhật trạng thái người dùng
        {
            try
            {
                var data = model.TaiKhoan.Find(id);
                if (data == null)
                    return Content("Người dùng không tồn tại trên hệ thống.");

                var aspUser = data.AspNetUsers;
                aspUser.LockoutEnabled = !trangthai;

                model.Entry(aspUser).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Mở form chỉnh sửa thông tin người dùng.
        /// </summary>
        /// <returns>Trả về partial view "_EditUser" với thông tin người dùng, hoặc thông báo lỗi nếu có vấn đề</returns>
        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult OpenEdit(int id) //Mở form cập nhật thông tin người dùng
        {
            try
            {
                var data = model.TaiKhoan.Find(id);
                if (data == null)
                    return Content("Người dùng không tồn tại trên hệ thống.");

                return PartialView("_EditUser", data);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Cập nhật thông tin người dùng và vai trò của họ.
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu thành công, "Exist" nếu có lỗi trùng lặp, hoặc thông báo lỗi chi tiết nếu gặp sự cố</returns>
        [Authorize, BCNRole]
        [HttpPost] //Lưu cập nhật thông tin người dùng
        public ActionResult SubmitEdit(int id, string ma, string hoten, string email, string chucdanh, string dienthoai, string nganh, string gioitinh, DateTime? ngaysinh)
        {
            try
            {
                var checks = model.TaiKhoan.FirstOrDefault(u => (u.Email.ToLower().Equals(email.ToLower()) || u.Ma.ToLower().Equals(ma.ToLower())) && u.ID != id);
                if (checks != null)
                    return Content("Exist");

                var aspNetRoles = model.AspNetRoles.Where(w => w.ID.Equals(chucdanh)).ToList();

                string userId = email.ToLower();
                var aspNetUsers = model.AspNetUsers.Find(userId);

                string idRole = aspNetUsers.AspNetRoles.First().ID;
                UserManager.RemoveFromRoles(userId, idRole);

                aspNetUsers.AspNetRoles = aspNetRoles;
                aspNetUsers.Email = email;
                aspNetUsers.UserName = hoten;
                model.Entry(aspNetUsers).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                var data = model.TaiKhoan.Find(id);
                data.Email = email;
                data.HoTen = hoten;
                data.NgaySinh = ngaysinh;
                data.Ma = ma;
                data.GioiTinh = gioitinh;
                data.SDT = dienthoai;
                if (!string.IsNullOrEmpty(nganh))
                {
                    data.ID_Nganh = Int32.Parse(nganh);
                    int khoa = model.Nganh.Find(Int32.Parse(nganh)).ID_Khoa;
                    data.ID_Khoa = khoa;
                }

                model.Entry(data).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        //[Authorize]
        //[BCNRole]
        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        var data = model.TaiKhoan.Find(id);
        //        if (data == null)
        //            return Content("SUCCESS");

        //        var aspUser = data.AspNetUsers;

        //        string userId = data.Email.ToLower();
        //        var aspNetUsers = model.AspNetUsers.Find(userId);

        //        string idRole = aspNetUsers.AspNetRoles.First().ID;
        //        UserManager.RemoveFromRoles(userId, idRole);

        //        var lhp = data.LopHocPhan.ToList();
        //        foreach (var item in lhp)
        //        {
        //            item.ID_TaiKhoan = null;
        //            model.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //        }
        //        model.SaveChanges();

        //        model.TaiKhoan.Remove(data);
        //        model.AspNetUsers.Remove(aspUser);
        //        model.SaveChanges();

        //        model = new CongTacTroGiangKhoaCNTTEntities();

        //        return Content("SUCCESS");
        //    }
        //    catch (Exception Ex)
        //    {
        //        return Content("Chi tiết lỗi: " + Ex.Message);
        //    }
        //}
        /// <summary>
        /// Lọc danh sách người dùng theo vai trò và/hoặc ngành của người dùng.
        /// </summary>
        /// <returns>Trả về partial view "_Filter" với danh sách người dùng đã lọc, hoặc thông báo lỗi nếu có sự cố</returns>
        [Authorize]
        [BCNRole]
        [HttpPost]
        public ActionResult Filter(string id) //Lọc danh sách người dùng theo role
        {
            try
            {
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;

                string userTennganh = taikhoan.Nganh == null ? "công nghệ thông tin" : taikhoan.Nganh.TenNganh.ToLower().ToLower();
                var userKhoa = model.Khoa.FirstOrDefault(f => f.Nganh.Where(w => w.TenNganh.Equals(userTennganh)).Count() > 0);
                var userNganh = userKhoa.Nganh.OrderByDescending(o => o.ID).ToList();

                var lstKhoa = model.Khoa.ToList();
                var lstNganh = model.Nganh.ToList();

                int roleId = Int32.Parse(Session["user-role-id"].ToString());

                if (string.IsNullOrEmpty(id) || id.Equals("0"))
                {
                    if (roleId == 3)
                    {
                        List<int> idNganh = new List<int>();
                        foreach (var item in userKhoa.Nganh.ToList())
                        {
                            idNganh.Add(item.ID);
                        }
                        var data = model.AspNetUsers.Where(w => w.TaiKhoan.Where(wt => wt.ID_Nganh != null).Count() > 0 && w.TaiKhoan.Where(wt => idNganh.Contains(wt.ID_Nganh.Value)).Count() > 0).ToList().OrderByDescending(o => o.ID);
                        return PartialView("_Filter", data);
                    }
                    else
                    {
                        var data = model.AspNetUsers.ToList().OrderByDescending(o => o.ID);
                        return PartialView("_Filter", data);
                    }
                }
                else
                {
                    if (roleId == 3)
                    {
                        List<int> idNganh = new List<int>();
                        foreach (var item in userKhoa.Nganh.ToList())
                        {
                            idNganh.Add(item.ID);
                        }
                        var data = model.AspNetUsers.Where(w => w.TaiKhoan.Where(wt => wt.ID_Nganh != null).Count() > 0 && w.TaiKhoan.Where(wt => idNganh.Contains(wt.ID_Nganh.Value)).Count() > 0
                        && w.AspNetRoles.Where(wr => wr.ID.Equals(id)).Count() > 0).ToList().OrderByDescending(o => o.ID);
                        return PartialView("_Filter", data);
                    }
                    else
                    {
                        var data = model.AspNetUsers.Where(w => w.AspNetRoles.Where(wr => wr.ID.Equals(id)).Count() > 0).ToList().OrderByDescending(o => o.ID);
                        return PartialView("_Filter", data);

                    }
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Cập nhật thông tin người dùng, bao gồm mã, điện thoại, ngành, giới tính, ngày sinh, tài khoản ngân hàng, căn cước công dân, mã số thuế và ghi chú.
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu thành công, "Exist" nếu có lỗi trùng mã, hoặc thông báo lỗi chi tiết nếu gặp sự cố</returns>
        [Authorize, AllRole]
        [HttpPost] //Cập nhật thông tin cá nhân của tài khoản
        public ActionResult UpdateInfo(string ma, string dienthoai, string nganh, string gioitinh, DateTime? ngaysinh,
            string sotaikhoan, string nganhang, string chutaikhoan, string cancuoc, string mst, string ghichu)
        {
            try
            {
                int id = Int32.Parse(Session["user-id"].ToString());

                if (!string.IsNullOrEmpty(ma))
                {
                    var checks = model.TaiKhoan.FirstOrDefault(u => u.Ma.ToLower().Equals(ma.ToLower()) && u.ID != id);
                    if (checks != null)
                        return Content("Exist");
                }

                var data = model.TaiKhoan.Find(id);
                data.NgaySinh = ngaysinh;

                int roleId = Int32.Parse(Session["user-role-id"].ToString());
                if (roleId != 4 && roleId != 1)
                    data.Ma = ma;

                data.GioiTinh = gioitinh;
                data.SDT = dienthoai;
                if (!string.IsNullOrEmpty(nganh))
                {
                    data.ID_Nganh = Int32.Parse(nganh);
                    int khoa = model.Nganh.Find(Int32.Parse(nganh)).ID_Khoa;
                    data.ID_Khoa = khoa;
                }

                data.SoTaiKhoanNganHang = sotaikhoan;
                data.TenNganHang = nganhang;
                data.ChuTaiKhoanNganHang = chutaikhoan;
                data.MaSoCanCuocCongDan = cancuoc;
                data.MaSoThue = mst;
                data.GhiChu = ghichu;

                model.Entry(data).State = System.Data.Entity.EntityState.Modified;
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