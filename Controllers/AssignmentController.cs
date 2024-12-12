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

        [Authorize, GVRole]
        public ActionResult Filter(int hocky, int nganh, string trangthai)
        {
            try
            {
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

        [Authorize, GVRole]
        public ActionResult LoadListTA(int lhp)
        {
            try
            {
                var lstTA = model.UngTuyenTroGiang.Where(w => w.ID_LopHocPhan == lhp && w.TrangThai == true).ToList();
                Session["title-update-assign"] = model.LopHocPhan.Find(lhp);
                return PartialView("_ListTA", lstTA);
            }
            catch (Exception Ex)
            {
                Session["title-update-assign"] = null;
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, GVRole]
        public ActionResult SubmitUpdateAssign(int idtk, int idlhp)
        {
            try
            {
                var lhp = model.LopHocPhan.Find(idlhp);
                if (lhp != null)
                {
                    var phancong = lhp.PhanCongTroGiang.First();
                    phancong.ID_TaiKhoan = idtk;

                    model.Entry(phancong).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

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
                    return Content("Chi tiết lỗi: Lớp học phần không tồn tại!");
                }
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}