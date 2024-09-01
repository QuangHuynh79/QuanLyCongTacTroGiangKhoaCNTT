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
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Areas.studentaffairs.Controllers
{
    public class studentaffairstimetableController : Controller
    {
        TrogiangvluEntities model = new TrogiangvluEntities();
        // GET: studentaffairs/timetable
        [Authorize]
        public ActionResult Index()
        {
            return View("index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Import()
        {
            return View("import");
        }

        [Authorize]
        [HttpPost]
        public ActionResult SubmitImport(HttpPostedFileBase fileImport, int hocky, int nganh, string confirm)
        {
            try
            {
                if (fileImport != null && fileImport.ContentLength > (1024 * 1024 * 50)) // 50MB limit
                {
                    return Content("more50mb");
                }
                else
                {
                    if (!string.IsNullOrEmpty(confirm))
                    {
                        if (confirm.Equals("addnew"))
                        {
                            var currentTkb = model.ThoiKhoaBieu.FirstOrDefault(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh);
                            model.ThoiKhoaBieu.Remove(currentTkb);
                            model.SaveChanges();

                            string filePath = string.Empty;
                            string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            filePath = path + Path.GetFileName(fileImport.FileName);
                            string extension = Path.GetExtension(fileImport.FileName);
                            fileImport.SaveAs(filePath);

                            string conString = string.Empty;
                            switch (extension)
                            {
                                case ".xls": //For Excel 97-03.
                                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                    break;
                                case ".xlsx": //For Excel 07 and above.
                                    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                    break;
                            }
                            DataTable dtExcel = new DataTable();

                            try
                            {
                                conString = string.Format(conString, filePath);

                                using (OleDbConnection connExcel = new OleDbConnection(conString))
                                {
                                    using (OleDbCommand cmdExcel = new OleDbCommand())
                                    {
                                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                        {
                                            cmdExcel.Connection = connExcel;

                                            //Get the name of First Sheet.
                                            connExcel.Open();
                                            DataTable dtExcelSchema;
                                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                            connExcel.Close();

                                            //Read Data from First Sheet.
                                            connExcel.Open();
                                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                            odaExcel.SelectCommand = cmdExcel;
                                            odaExcel.Fill(dtExcel);
                                            connExcel.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return Json("error" + e.Message);
                            }

                            DataRow dr = dtExcel.Rows[0];
                            if (dr.Table.Columns.Count != 32)
                                return Content("INCORRECT");

                            dr.Delete();
                            dtExcel.AcceptChanges();

                            List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                            var lstTkb = new List<ThoiKhoaBieu>();
                            foreach (DataRow data in dtExcel.Rows)
                            {




                                var malop = "";
                                foreach (var item in data[6].ToString().Split('\n'))
                                {
                                    malop += item + "#";
                                }
                                malop = malop.Substring(0, malop.Length);

                                var tkb = new ThoiKhoaBieu();
                                tkb.ID_HocKy = hocky;
                                tkb.ID_Nganh = nganh;
                                tkb.MaGocLHP = data[0].ToString();
                                tkb.MaMH = data[1].ToString();
                                tkb.MaLHP = data[2].ToString();
                                tkb.TenHP = data[3].ToString();
                                tkb.SoTC = data[4].ToString();
                                tkb.LoaiHP = data[5].ToString();
                                tkb.MaLop = malop;
                                tkb.TSMH = data[7].ToString();
                                tkb.SoTietDaXep = data[8].ToString();
                                tkb.PH = data[9].ToString();
                                tkb.Thu = data[10].ToString();
                                tkb.TietBD = data[11].ToString();
                                tkb.SoTiet = data[12].ToString();
                                tkb.TietHoc = data[13].ToString();
                                tkb.Phong = data[14].ToString();
                                tkb.MaCBGD = data[15].ToString();
                                tkb.TenCBGD = data[16].ToString();
                                tkb.PH_X = data[17].ToString();
                                tkb.SucChua = data[18].ToString();
                                tkb.SiSoTKB = data[19].ToString();
                                tkb.Trong = data[20].ToString();
                                tkb.TinhTrangLHP = data[21].ToString();
                                tkb.TuanHoc2 = data[22].ToString();
                                tkb.ThuS = data[23].ToString();
                                tkb.TietS = data[24].ToString();
                                tkb.SoSVDK = data[25].ToString();
                                tkb.TuanBD = data[26].ToString();
                                tkb.TuanKT = data[27].ToString();
                                tkb.MaNganh = data[28].ToString();
                                tkb.TenNganh = data[29].ToString();
                                tkb.GhiChu1 = data[30].ToString();
                                tkb.GhiChu2 = data[31].ToString();

                                lstTkb.Add(tkb);

                            }
                            model.ThoiKhoaBieu.AddRange(lstTkb);
                            model.SaveChanges();
                        }
                        else //replace
                        {
                            var tkb = model.ThoiKhoaBieu.FirstOrDefault(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh);

                            string filePath = string.Empty;
                            string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            filePath = path + Path.GetFileName(fileImport.FileName);
                            string extension = Path.GetExtension(fileImport.FileName);
                            fileImport.SaveAs(filePath);

                            string conString = string.Empty;
                            switch (extension)
                            {
                                case ".xls": //For Excel 97-03.
                                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                    break;
                                case ".xlsx": //For Excel 07 and above.
                                    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                    break;
                            }
                            DataTable dtExcel = new DataTable();

                            try
                            {
                                conString = string.Format(conString, filePath);

                                using (OleDbConnection connExcel = new OleDbConnection(conString))
                                {
                                    using (OleDbCommand cmdExcel = new OleDbCommand())
                                    {
                                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                        {
                                            cmdExcel.Connection = connExcel;

                                            //Get the name of First Sheet.
                                            connExcel.Open();
                                            DataTable dtExcelSchema;
                                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                            connExcel.Close();

                                            //Read Data from First Sheet.
                                            connExcel.Open();
                                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                            odaExcel.SelectCommand = cmdExcel;
                                            odaExcel.Fill(dtExcel);
                                            connExcel.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return Json("error" + e.Message);
                            }

                            DataRow dr = dtExcel.Rows[0];
                            if (dr.Table.Columns.Count != 32)
                                return Content("INCORRECT");

                            dr.Delete();
                            dtExcel.AcceptChanges();

                            List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                            var lstTkb = new List<ThoiKhoaBieu>();
                            foreach (DataRow data in dtExcel.Rows)
                            {
                                var malop = "";
                                foreach (var item in data[6].ToString().Split('\n'))
                                {
                                    malop += item.Trim() + "#";
                                }
                                malop = malop.Substring(0, malop.Length);

                                if (!tkb.MaGocLHP.Equals(data[0].ToString().ToLower().Trim()))
                                    tkb.MaGocLHP = data[0].ToString().Trim();
                                if (!tkb.MaMH.Equals(data[1].ToString().ToLower().Trim()))
                                    tkb.MaMH = data[1].ToString().Trim();
                                if (!tkb.MaLHP.Equals(data[2].ToString().ToLower().Trim()))
                                    tkb.MaLHP = data[2].ToString().Trim();
                                if (!tkb.TenHP.ToLower().Equals(data[3].ToString().ToLower().Trim()))
                                    tkb.TenHP = data[3].ToString().Trim();
                                if (!tkb.SoTC.ToLower().Equals(data[4].ToString().ToLower().Trim()))
                                    tkb.SoTC = data[4].ToString().Trim();
                                if (!tkb.LoaiHP.ToLower().Equals(data[5].ToString().ToLower().Trim()))
                                    tkb.LoaiHP = data[5].ToString().Trim();
                                if (!tkb.MaLop.ToLower().Equals(malop.ToLower().Trim()))
                                    tkb.MaLop = malop;
                                if (!tkb.TSMH.ToLower().Equals(data[7].ToString().ToLower().Trim()))
                                    tkb.TSMH = data[7].ToString().Trim();
                                if (!tkb.SoTietDaXep.ToLower().Equals(data[8].ToString().ToLower().Trim()))
                                    tkb.SoTietDaXep = data[8].ToString().Trim();
                                if (!tkb.PH.ToLower().Equals(data[9].ToString().ToLower().Trim()))
                                    tkb.PH = data[9].ToString().Trim();
                                if (!tkb.Thu.ToLower().Equals(data[10].ToString().ToLower().Trim()))
                                    tkb.Thu = data[10].ToString().Trim();
                                if (!tkb.TietBD.ToLower().Equals(data[11].ToString().ToLower().Trim()))
                                    tkb.TietBD = data[11].ToString().Trim();
                                if (!tkb.SoTiet.ToLower().Equals(data[12].ToString().ToLower().Trim()))
                                    tkb.SoTiet = data[12].ToString().Trim();
                                if (!tkb.TietHoc.ToLower().Equals(data[13].ToString().ToLower().Trim()))
                                    tkb.TietHoc = data[13].ToString().Trim();
                                if (!tkb.Phong.ToLower().Equals(data[14].ToString().ToLower().Trim()))
                                    tkb.Phong = data[14].ToString().Trim();
                                if (!tkb.MaCBGD.ToLower().Equals(data[15].ToString().ToLower().Trim()))
                                    tkb.MaCBGD = data[15].ToString().Trim();
                                if (!tkb.TenCBGD.ToLower().Equals(data[16].ToString().ToLower().Trim()))
                                    tkb.TenCBGD = data[16].ToString().Trim();
                                if (!tkb.PH_X.ToLower().Equals(data[17].ToString().ToLower().Trim()))
                                    tkb.PH_X = data[17].ToString().Trim();
                                if (!tkb.SucChua.ToLower().Equals(data[18].ToString().ToLower().Trim()))
                                    tkb.SucChua = data[18].ToString().Trim();
                                if (!tkb.SiSoTKB.ToLower().Equals(data[19].ToString().ToLower().Trim()))
                                    tkb.SiSoTKB = data[19].ToString().Trim();
                                if (!tkb.Trong.ToLower().Equals(data[20].ToString().ToLower().Trim()))
                                    tkb.Trong = data[20].ToString().Trim();
                                if (!tkb.TinhTrangLHP.ToLower().Equals(data[21].ToString().ToLower().Trim()))
                                    tkb.TinhTrangLHP = data[21].ToString().Trim();
                                if (!tkb.TuanHoc2.ToLower().Equals(data[22].ToString().ToLower().Trim()))
                                    tkb.TuanHoc2 = data[22].ToString().Trim();
                                if (!tkb.ThuS.ToLower().Equals(data[23].ToString().ToLower().Trim()))
                                    tkb.ThuS = data[23].ToString().Trim();
                                if (!tkb.TietS.ToLower().Equals(data[24].ToString().ToLower().Trim()))
                                    tkb.TietS = data[24].ToString().Trim();
                                if (!tkb.SoSVDK.ToLower().Equals(data[25].ToString().ToLower().Trim()))
                                    tkb.SoSVDK = data[25].ToString().Trim();
                                if (!tkb.TuanBD.ToLower().Equals(data[26].ToString().ToLower().Trim()))
                                    tkb.TuanBD = data[26].ToString().Trim();
                                if (!tkb.TuanKT.ToLower().Equals(data[27].ToString().ToLower().Trim()))
                                    tkb.TuanKT = data[27].ToString().Trim();
                                if (!tkb.MaNganh.ToLower().Equals(data[28].ToString().ToLower().Trim()))
                                    tkb.MaNganh = data[28].ToString().Trim();
                                if (!tkb.TenNganh.ToLower().Equals(data[29].ToString().ToLower().Trim()))
                                    tkb.TenNganh = data[29].ToString().Trim();
                                if (!tkb.GhiChu1.ToLower().Equals(data[30].ToString().ToLower().Trim()))
                                    tkb.GhiChu1 = data[30].ToString().Trim();
                                if (!tkb.GhiChu2.ToLower().Equals(data[31].ToString().ToLower().Trim()))
                                    tkb.GhiChu2 = data[31].ToString().Trim();

                                lstTkb.Add(tkb);
                            }
                            model.Entry(tkb).State = System.Data.Entity.EntityState.Modified;
                            model.SaveChanges();
                        }
                    }
                    else
                    {
                        var checkExist = model.ThoiKhoaBieu.FirstOrDefault(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh);
                        if (checkExist != null)
                            return Content("Exist");

                        string filePath = string.Empty;
                        string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + Path.GetFileName(fileImport.FileName);
                        string extension = Path.GetExtension(fileImport.FileName);
                        fileImport.SaveAs(filePath);

                        string conString = string.Empty;
                        switch (extension)
                        {
                            case ".xls": //For Excel 97-03.
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx": //For Excel 07 and above.
                                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                break;
                        }
                        DataTable dtExcel = new DataTable();

                        try
                        {
                            conString = string.Format(conString, filePath);

                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.
                                        connExcel.Open();
                                        DataTable dtExcelSchema;
                                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                        connExcel.Close();

                                        //Read Data from First Sheet.
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dtExcel);
                                        connExcel.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            return Json("error" + e.Message);
                        }

                        DataRow dr = dtExcel.Rows[0];
                        if (dr.Table.Columns.Count != 32)
                            return Content("INCORRECT");

                        dr.Delete();
                        dtExcel.AcceptChanges();

                        List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                        var lstTkb = new List<ThoiKhoaBieu>();
                        foreach (DataRow data in dtExcel.Rows)
                        {
                            var malop = "";
                            foreach (var item in data[6].ToString().Split('\n'))
                            {
                                malop += item + "#";
                            }
                            malop = malop.Substring(0, malop.Length);

                            var tkb = new ThoiKhoaBieu();
                            tkb.ID_HocKy = hocky;
                            tkb.ID_Nganh = nganh;
                            tkb.MaGocLHP = data[0].ToString();
                            tkb.MaMH = data[1].ToString();
                            tkb.MaLHP = data[2].ToString();
                            tkb.TenHP = data[3].ToString();
                            tkb.SoTC = data[4].ToString();
                            tkb.LoaiHP = data[5].ToString();
                            tkb.MaLop = malop;
                            tkb.TSMH = data[7].ToString();
                            tkb.SoTietDaXep = data[8].ToString();
                            tkb.PH = data[9].ToString();
                            tkb.Thu = data[10].ToString();
                            tkb.TietBD = data[11].ToString();
                            tkb.SoTiet = data[12].ToString();
                            tkb.TietHoc = data[13].ToString();
                            tkb.Phong = data[14].ToString();
                            tkb.MaCBGD = data[15].ToString();
                            tkb.TenCBGD = data[16].ToString();
                            tkb.PH_X = data[17].ToString();
                            tkb.SucChua = data[18].ToString();
                            tkb.SiSoTKB = data[19].ToString();
                            tkb.Trong = data[20].ToString();
                            tkb.TinhTrangLHP = data[21].ToString();
                            tkb.TuanHoc2 = data[22].ToString();
                            tkb.ThuS = data[23].ToString();
                            tkb.TietS = data[24].ToString();
                            tkb.SoSVDK = data[25].ToString();
                            tkb.TuanBD = data[26].ToString();
                            tkb.TuanKT = data[27].ToString();
                            tkb.MaNganh = data[28].ToString();
                            tkb.TenNganh = data[29].ToString();
                            tkb.GhiChu1 = data[30].ToString();
                            tkb.GhiChu2 = data[31].ToString();

                            lstTkb.Add(tkb);

                        }
                        model.ThoiKhoaBieu.AddRange(lstTkb);
                        model.SaveChanges();
                    }
                    return Content("SUCCESS");
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ContentResult DownloadFile(string fileName)
        {
            fileName = "CNTT UIS-ThoiKhoaBieu_TieuChuan_Mau.xlsx";

            string path = Server.MapPath("~/Content/timetable/formimport/");
            byte[] bytes = System.IO.File.ReadAllBytes(path + fileName);
            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

            return Content(base64);
        }
    }
}