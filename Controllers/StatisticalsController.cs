using QuanLyCongTacTroGiangKhoaCNTT.Models;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class StatisticalsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này trả về view "Statistical" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Statistical" với số liệu thống kê TA.</returns>
        [Authorize, BCNRole]
        public ActionResult Statistical()
        {
            return View("Statistical", model.UngTuyenTroGiang.ToList());
        }

        /// <summary>
        /// Hàm này trả về dữ liệu được lọc view "Statistical" cho người dùng.
        /// </summary>
        /// <returns>Trả về View "Statistical" với số liệu thống kê TA được lọc.</returns>
        [Authorize, BCNRole]
        public ActionResult FilterStatistical(int hocky, int nganh)
        {
            var data = model.UngTuyenTroGiang.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).ToList();
            if (data.Count > 0) {
                var dataGroupBy = data.Where(w => w.TaiKhoan.ID_Nganh == nganh && w.LopHocPhan.ID_HocKy == hocky).GroupBy(g => g.ID_TaiKhoan).ToList();
            }
            return PartialView("_FilterStatistical", data);
        }
    }
}