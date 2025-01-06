using DocumentFormat.OpenXml.EMMA;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class ResultApplyTeachingAssistantController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Lấy kết quả đăng ký trợ giảng của người dùng dựa trên tài khoản hiện tại.
        /// Hàm này truy vấn dữ liệu ứng tuyển trợ giảng của người dùng và trả về danh sách kết quả.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "ResultApply", chứa danh sách các kết quả đăng ký trợ giảng của người dùng, được lấy từ cơ sở dữ liệu.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult ResultApply() //Xem kết quả đăng ký trợ giảng
        {
            var idtk = Int32.Parse(Session["user-id"].ToString());
            var ut = model.UngTuyenTroGiang.Where(w => w.ID_TaiKhoan == idtk).ToList();
            return View("ResultApply", ut);
        }
        /// <summary>
        /// Lọc và lấy kết quả đăng ký trợ giảng của người dùng theo học kỳ và ngành.
        /// Hàm này truy vấn dữ liệu từ bảng đăng ký trợ giảng dựa trên học kỳ và ngành, sau đó lọc ra các ứng tuyển của người dùng theo tài khoản.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với tên "_FilterResultApply", chứa danh sách các kết quả đăng ký trợ giảng của người dùng trong học kỳ và ngành đã chọn.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult FilterResultApply(int hocky, int nganh) //Xem kết quả đăng ký trợ giảng
        {
            var idtk = Int32.Parse(Session["user-id"].ToString());
            var formDky = model.FormDangKyTroGiang.Where(f => f.ID_HocKy == hocky && f.ID_Nganh == nganh).ToList();
            List<int> idForm = new List<int>();
            foreach (var item in formDky)
                idForm.Add(item.ID);

            var ut = model.UngTuyenTroGiang.Where(w => w.ID_TaiKhoan == idtk && idForm.Contains(w.ID_FormDangKyTroGiang)).ToList();
            return PartialView("_FilterResultApply", ut);
        }
    }
}