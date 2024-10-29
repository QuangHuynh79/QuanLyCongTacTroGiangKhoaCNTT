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
        [Authorize, GVandBCNRole]
        public ActionResult Index()
        {
            return View("Index");
        }

        [Authorize, GVandBCNRole]
        [HttpPost]
        public ActionResult FilterData(int hocky, int nganh, string trangthai)
        {
            int idForm = 0;
            var form = model.FormDangKyTroGiang.FirstOrDefault(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh);
            if (form != null)
                idForm = form.ID;

            if (trangthai.Equals("all"))
            {
                var uts1 = model.UngTuyenTroGiang.Where(w => w.ID_FormDangKyTroGiang == idForm && w.TrangThai == true).ToList().OrderByDescending(o => o.ID);
                var ut = uts1.Where(w => w.DanhGiaPhongVan.Where(wd => wd.KetLuanDat == true).Count() > 0).ToList();

                return PartialView("_FilterCongViec", ut);
            }
            else
            {
                var blTrangthai = Convert.ToBoolean(trangthai);

                var uts1 = model.UngTuyenTroGiang.Where(w => w.ID_FormDangKyTroGiang == idForm && w.TrangThai == true).ToList().OrderByDescending(o => o.ID);
                var uts2 = uts1.Where(w => w.DanhGiaPhongVan.Where(wd => wd.KetLuanDat == true).Count() > 0).ToList();
                var ut = uts2.Where(w => w.LopHocPhan.PhanCongTroGiang.Where(wS => wS.TrangThai == blTrangthai).Count() > 0).ToList();

                return PartialView("_FilterCongViec", ut);
            }
        }

        [Authorize, GVandBCNRole]
        [HttpPost]
        public ActionResult OpenReviewTask(int id, string type)
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
        public ActionResult SubmitReviewTask(string lstid, string lsttrangthai, string ghichu, string giothucte)
        {
            try
            {
                var id = lstid.Split('#');
                var trangthai = lsttrangthai.Split('#');

                int idPc = Convert.ToInt32(id[0]); 
                var pc = model.CongViec.Find(idPc).LopHocPhan.PhanCongTroGiang.First();
                pc.TrangThai = true;
                pc.SoGioThucTe = float.Parse(giothucte.Replace(".", ","));
                pc.GhiChu = ghichu;
                model.Entry(pc).State = System.Data.Entity.EntityState.Modified;

                for (int i = 0; i < id.Length; i++) { 
                    int idCv = Convert.ToInt32(id[i]);
                    var congviec = model.CongViec.Find(idCv);

                    if(Convert.ToBoolean(trangthai[i]))
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