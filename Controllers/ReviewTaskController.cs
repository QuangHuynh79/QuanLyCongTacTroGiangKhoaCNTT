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
        [Authorize, GVandBCNandTARole]
        public ActionResult Index() // Xem danh sách đánh giá công việc lớp học phần
        {
            return View("Index");
        }

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

        [Authorize, GVandBCNandTARole]
        [HttpPost]
        public ActionResult OpenReviewTask(int id, string type) //Mở chi tiết đánh giá lớp học phần
        {
            if (type.Equals("edit"))
            {
                return PartialView("_OpenEditReviewTask", model.LopHocPhan.Find(id));
            }
            else
            {
                return PartialView("_OpenViewReviewTask", model.LopHocPhan.Find(id));
            }
        }

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