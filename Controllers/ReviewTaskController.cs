using Microsoft.AspNet.Identity;
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
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class ReviewTaskController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: TaskList
        /// <summary>
        /// Hiển thị danh sách đánh giá công việc của lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ViewResult"> với View "Index" để hiển thị trang danh sách đánh giá công việc lớp học phần.
        /// </returns>
        [Authorize, GVandBCNandTARole]
        public ActionResult Index() // Xem danh sách đánh giá công việc lớp học phần
        {
            return View("Index");
        }
        /// <summary>
        /// Lọc danh sách lớp học phần dựa trên học kỳ, ngành và trạng thái của phân công trợ giảng.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="PartialViewResult"> hiển thị danh sách lớp học phần đã lọc.
        /// </returns>
        [Authorize, GVandBCNandTARole]
        [HttpPost]
        public ActionResult FilterData(int hocky, int nganh, string trangthai) //Lọc danh sách đánh giá lhp
        {
            var lhp = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.PhanCongTroGiang.Count() > 0).ToList().OrderByDescending(o => o.ID).ToList();

            if (trangthai.Equals("all"))
            {
                return PartialView("_FilterCongViec", lhp);
            }
            else
            {
                var blTrangthai = Convert.ToBoolean(trangthai);

                var lhps = lhp.Where(w => w.PhanCongTroGiang.Where(wp => wp.TrangThai == blTrangthai).Count() > 0).ToList().OrderByDescending(o => o.ID).ToList();
                return PartialView("_FilterCongViec", lhps);
            }
        }
        /// <summary>
        /// Mở chi tiết đánh giá của lớp học phần dựa trên ID lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="PartialViewResult"> hiển thị chi tiết đánh giá của lớp học phần.
        /// </returns>
        [Authorize, GVandBCNandTARole]
        [HttpPost]
        public ActionResult OpenReviewTask(int id) //Mở chi tiết đánh giá lớp học phần
        {
            return PartialView("_OpenEditReviewTask", model.LopHocPhan.Find(id));
        }
        /// <summary>
        /// Lưu thông tin đánh giá lớp học phần, bao gồm trạng thái công việc, ghi chú và số giờ thực tế.
        /// </summary>
        /// <returns>
        /// Trả về thông báo "SUCCESS" nếu lưu thành công, hoặc chi tiết lỗi nếu có ngoại lệ.
        /// </returns>
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult SubmitReviewTask(string lstid, string lsttrangthai, string ghichu, string giothucte) //Lưu thông tin đánh giá lhp
        {
            try
            {
                var id = lstid.Split('#');
                var trangthai = lsttrangthai.Split('#');

                int idPc = Convert.ToInt32(id[0]);
                var pc = model.CongViec.Find(idPc).LopHocPhan.PhanCongTroGiang.First();
                pc.TrangThai = true;
                pc.SoGioThucTe = float.Parse(giothucte);
                pc.GhiChu = ghichu;
                model.Entry(pc).State = System.Data.Entity.EntityState.Modified;

                for (int i = 0; i < id.Length; i++)
                {
                    int idCv = Convert.ToInt32(id[i]);
                    var congviec = model.CongViec.Find(idCv);

                    if (Convert.ToBoolean(trangthai[i]))
                    {
                        congviec.KetQuaCongViec = "hoanthanh";
                    }
                    else
                    {
                        congviec.KetQuaCongViec = "chuahoanthanh";
                    }
                    model.Entry(congviec).State = System.Data.Entity.EntityState.Modified;
                }
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();
                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}