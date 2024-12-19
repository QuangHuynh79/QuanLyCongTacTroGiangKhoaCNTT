using QuanLyCongTacTroGiangKhoaCNTT.Models;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class StatisticalsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này trả về view "Statistical" thống kê trợ giảng cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Statistical" với số liệu thống kê TA.</returns>
        [Authorize, BCNRole]
        public ActionResult Statistical()
        {
            return View("Statistical", model.UngTuyenTroGiang.ToList());
        }

        /// <summary>
        /// Hàm này trả về view "Statistical" thống kê thù lao cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Statistical" với số liệu thống kê thù lao của TA.</returns>
        [Authorize, BCNRole]
        public ActionResult StatisticalRemuneration()
        {
            return View("StatisticalRemuneration", model.UngTuyenTroGiang.ToList());
        }
       
        /// <summary>
        /// Hàm này trả về dữ liệu được lọc view "Statistical" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Statistical" với số liệu thống kê TA được lọc.</returns>
        [Authorize, BCNRole]
        public ActionResult FilterStatistical(int hocky, int nganh)
        {
            var data = model.UngTuyenTroGiang.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).ToList();
            if (data.Count > 0)
            {
                var dataGroupBy = data.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).GroupBy(g => g.ID_TaiKhoan).ToList();
            }
            return PartialView("_FilterStatistical", data);
        }

        /// <summary>
        /// Hàm này trả về dữ liệu được lọc view "FilterRemuneration" thống kê thù lao cho người dùng.
        /// </summary>
        /// <returns>Trả về View "FilterRemuneration" với số liệu thống kê thù lao được lọc.</returns>
        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult FilterRemuneration(int hocky, int nganh)
        {
            var data = model.UngTuyenTroGiang.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).ToList();
            if (data.Count > 0)
            {
                var dataGroupBy = data.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).GroupBy(g => g.ID_TaiKhoan).ToList();
            }
            return PartialView("_FilterRemuneration", data);
        }

        /// <summary>
        /// Hàm này dùng để lưu cập nhật giá thù lao.
        /// </summary>
        /// <returns>Trả về trạng thái đã lưu thành công hay lỗi thất bại.</returns>
        [Authorize, BCNRole]
        public ActionResult UpdateRemuneratiion(string thulao)
        {
            try
            {
                Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");
                if (objAppsettings != null)
                {
                    objAppsettings.Settings["RemunerationPrice"].Value = thulao;
                    objConfig.Save();
                }
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi " + Ex.Message);
            }

        }
    }
}