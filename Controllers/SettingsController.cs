using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using System.Configuration;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class SettingsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();


        /// <summary>
        /// Hàm này dùng để trả về giao diện cài đặt nội dung gửi mail và định giá thù lao TA.
        /// </summary>
        /// <returns>Trả về View Setting.</returns>
        [Authorize, BCNRole]
        public ActionResult Setting()
        {
            var settingMail = model.ThongBaoMail.ToList();
            return View("Setting", settingMail);
        }

        /// <summary>
        /// Hàm này dùng để lưu cập nhật giá thù lao.
        /// </summary>
        /// <returns>Trả về trạng thái đã lưu thành công hay lỗi thất bại.</returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult UpdateRemuneratiion(string thulao, string giotoida)
        {
            try
            {
                Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");
                if (objAppsettings != null)
                {
                    objAppsettings.Settings["RemunerationPrice"].Value = thulao;
                    objAppsettings.Settings["RemunerationMaxHouse"].Value = giotoida;

                    objConfig.Save();
                }
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi " + Ex.Message);
            }

        }

        /// <summary>
        /// Hàm này dùng để lưu cập nhật thông tin gửi mail tự động.
        /// </summary>
        /// <returns>Trả về trạng thái đã lưu thành công hay lỗi thất bại.</returns>
        [Authorize, BCNRole]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateMailNotification(int id, string tieude, string noidung)
        {
            try
            {
                var mail = model.ThongBaoMail.Find(id);
                if (mail != null)
                {
                    mail.TieuDe = tieude;
                    mail.NoiDung = noidung;
                    mail.SuaDoiCuoi = DateTime.Now;
                    mail.NguoiSuaDoi = Int32.Parse(Session["user-id"].ToString());

                    model.Entry(mail).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi " + Ex.Message);
            }

        }

        /// <summary>
        /// Hàm này dùng để lọc loại mail tự động.
        /// </summary>
        /// <returns>Trả về thông tin mail tự động theo loại.</returns>
        [Authorize, BCNRole]
        public ActionResult FilterMailNotification(int id)
        {
            try
            {
                var mail = model.ThongBaoMail.Find(id);
                return PartialView("_Filter", mail);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi " + Ex.Message);
            }

        }
    }
}