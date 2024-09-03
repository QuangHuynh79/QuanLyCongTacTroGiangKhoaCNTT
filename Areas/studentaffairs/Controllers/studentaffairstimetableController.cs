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
                    // Check if term status is false to prevent assignments
                    var hockydb = model.HocKy.Find(hocky);
                    if (hockydb.TrangThai == false)
                        return Content("Close");

                    if (!string.IsNullOrEmpty(confirm))
                    {
                        if (confirm.Equals("replace"))
                        {
                            var currentTkb = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.ThoiKhoaBieu.RemoveRange(currentTkb);
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

                            string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                            DataTable dt = new DataTable();
                            conString = string.Format(conString, filePath);

                            try
                            {
                                using (OleDbConnection connExcel = new OleDbConnection(conString))
                                {
                                    using (OleDbCommand cmdExcel = new OleDbCommand())
                                    {
                                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                        {
                                            cmdExcel.Connection = connExcel;

                                            //Get the name of First Sheet.
                                            connExcel.Open();
                                            DataTable dtSchema;
                                            dtSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                            string sheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString();
                                            connExcel.Close();

                                            //Read Data from First Sheet.
                                            connExcel.Open();
                                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                            odaExcel.SelectCommand = cmdExcel;
                                            odaExcel.Fill(dt);
                                            connExcel.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return Json("error" + e.Message);
                            }

                            // Trim column name string
                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            // Validate all columns
                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                            var lstTkb = new List<ThoiKhoaBieu>();
                            foreach (DataRow data in dt.Rows)
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
                            var tkb = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();

                            string filePath = string.Empty;
                            string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            filePath = path + Path.GetFileName(fileImport.FileName);
                            string extension = Path.GetExtension(fileImport.FileName);
                            fileImport.SaveAs(filePath);

                            string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                            DataTable dt = new DataTable();
                            conString = string.Format(conString, filePath);

                            try
                            {
                                using (OleDbConnection connExcel = new OleDbConnection(conString))
                                {
                                    using (OleDbCommand cmdExcel = new OleDbCommand())
                                    {
                                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                        {
                                            cmdExcel.Connection = connExcel;

                                            //Get the name of First Sheet.
                                            connExcel.Open();
                                            DataTable dtSchema;
                                            dtSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                            string sheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString();
                                            connExcel.Close();

                                            //Read Data from First Sheet.
                                            connExcel.Open();
                                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                            odaExcel.SelectCommand = cmdExcel;
                                            odaExcel.Fill(dt);
                                            connExcel.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return Json("error" + e.Message);
                            }

                            // Trim column name string
                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            // Validate all columns
                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            var rowcount = tkb.Count();
                            if (rowcount < dt.Rows.Count)
                            {
                                List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();
                                var lstTkb = new List<ThoiKhoaBieu>();
                                for (int i = 0; i < rowcount; i++)
                                {
                                    DataRow data = dt.Rows[i];
                                    var malop = "";
                                    foreach (var item in data[6].ToString().Split('\n'))
                                    {
                                        malop += item.Trim() + "#";
                                    }
                                    malop = malop.Substring(0, malop.Length);

                                    if (!tkb[i].MaGocLHP.Equals(data[0].ToString().ToLower().Trim()))
                                        tkb[i].MaGocLHP = data[0].ToString().Trim();
                                    if (!tkb[i].MaMH.Equals(data[1].ToString().ToLower().Trim()))
                                        tkb[i].MaMH = data[1].ToString().Trim();
                                    if (!tkb[i].MaLHP.Equals(data[2].ToString().ToLower().Trim()))
                                        tkb[i].MaLHP = data[2].ToString().Trim();
                                    if (!tkb[i].TenHP.ToLower().Equals(data[3].ToString().ToLower().Trim()))
                                        tkb[i].TenHP = data[3].ToString().Trim();
                                    if (!tkb[i].SoTC.ToLower().Equals(data[4].ToString().ToLower().Trim()))
                                        tkb[i].SoTC = data[4].ToString().Trim();
                                    if (!tkb[i].LoaiHP.ToLower().Equals(data[5].ToString().ToLower().Trim()))
                                        tkb[i].LoaiHP = data[5].ToString().Trim();
                                    if (!tkb[i].MaLop.ToLower().Equals(malop.ToLower().Trim()))
                                        tkb[i].MaLop = malop;
                                    if (!tkb[i].TSMH.ToLower().Equals(data[7].ToString().ToLower().Trim()))
                                        tkb[i].TSMH = data[7].ToString().Trim();
                                    if (!tkb[i].SoTietDaXep.ToLower().Equals(data[8].ToString().ToLower().Trim()))
                                        tkb[i].SoTietDaXep = data[8].ToString().Trim();
                                    if (!tkb[i].PH.ToLower().Equals(data[9].ToString().ToLower().Trim()))
                                        tkb[i].PH = data[9].ToString().Trim();
                                    if (!tkb[i].Thu.ToLower().Equals(data[10].ToString().ToLower().Trim()))
                                        tkb[i].Thu = data[10].ToString().Trim();
                                    if (!tkb[i].TietBD.ToLower().Equals(data[11].ToString().ToLower().Trim()))
                                        tkb[i].TietBD = data[11].ToString().Trim();
                                    if (!tkb[i].SoTiet.ToLower().Equals(data[12].ToString().ToLower().Trim()))
                                        tkb[i].SoTiet = data[12].ToString().Trim();
                                    if (!tkb[i].TietHoc.ToLower().Equals(data[13].ToString().ToLower().Trim()))
                                        tkb[i].TietHoc = data[13].ToString().Trim();
                                    if (!tkb[i].Phong.ToLower().Equals(data[14].ToString().ToLower().Trim()))
                                        tkb[i].Phong = data[14].ToString().Trim();
                                    if (!tkb[i].MaCBGD.ToLower().Equals(data[15].ToString().ToLower().Trim()))
                                        tkb[i].MaCBGD = data[15].ToString().Trim();
                                    if (!tkb[i].TenCBGD.ToLower().Equals(data[16].ToString().ToLower().Trim()))
                                        tkb[i].TenCBGD = data[16].ToString().Trim();
                                    if (!tkb[i].PH_X.ToLower().Equals(data[17].ToString().ToLower().Trim()))
                                        tkb[i].PH_X = data[17].ToString().Trim();
                                    if (!tkb[i].SucChua.ToLower().Equals(data[18].ToString().ToLower().Trim()))
                                        tkb[i].SucChua = data[18].ToString().Trim();
                                    if (!tkb[i].SiSoTKB.ToLower().Equals(data[19].ToString().ToLower().Trim()))
                                        tkb[i].SiSoTKB = data[19].ToString().Trim();
                                    if (!tkb[i].Trong.ToLower().Equals(data[20].ToString().ToLower().Trim()))
                                        tkb[i].Trong = data[20].ToString().Trim();
                                    if (!tkb[i].TinhTrangLHP.ToLower().Equals(data[21].ToString().ToLower().Trim()))
                                        tkb[i].TinhTrangLHP = data[21].ToString().Trim();
                                    if (!tkb[i].TuanHoc2.ToLower().Equals(data[22].ToString().ToLower().Trim()))
                                        tkb[i].TuanHoc2 = data[22].ToString().Trim();
                                    if (!tkb[i].ThuS.ToLower().Equals(data[23].ToString().ToLower().Trim()))
                                        tkb[i].ThuS = data[23].ToString().Trim();
                                    if (!tkb[i].TietS.ToLower().Equals(data[24].ToString().ToLower().Trim()))
                                        tkb[i].TietS = data[24].ToString().Trim();
                                    if (!tkb[i].SoSVDK.ToLower().Equals(data[25].ToString().ToLower().Trim()))
                                        tkb[i].SoSVDK = data[25].ToString().Trim();
                                    if (!tkb[i].TuanBD.ToLower().Equals(data[26].ToString().ToLower().Trim()))
                                        tkb[i].TuanBD = data[26].ToString().Trim();
                                    if (!tkb[i].TuanKT.ToLower().Equals(data[27].ToString().ToLower().Trim()))
                                        tkb[i].TuanKT = data[27].ToString().Trim();
                                    if (!tkb[i].MaNganh.ToLower().Equals(data[28].ToString().ToLower().Trim()))
                                        tkb[i].MaNganh = data[28].ToString().Trim();
                                    if (!tkb[i].TenNganh.ToLower().Equals(data[29].ToString().ToLower().Trim()))
                                        tkb[i].TenNganh = data[29].ToString().Trim();
                                    if (!tkb[i].GhiChu1.ToLower().Equals(data[30].ToString().ToLower().Trim()))
                                        tkb[i].GhiChu1 = data[30].ToString().Trim();
                                    if (!tkb[i].GhiChu2.ToLower().Equals(data[31].ToString().ToLower().Trim()))
                                        tkb[i].GhiChu2 = data[31].ToString().Trim();

                                    model.Entry(tkb[i]).State = System.Data.Entity.EntityState.Modified;
                                    lstTkb.Add(tkb[i]);
                                }
                                model.SaveChanges();
                            }
                            else
                            {
                                List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();
                                int i = 0;
                                var lstTkb = new List<ThoiKhoaBieu>();
                                foreach (DataRow data in dt.Rows)
                                {
                                    var malop = "";
                                    foreach (var item in data[6].ToString().Split('\n'))
                                    {
                                        malop += item.Trim() + "#";
                                    }
                                    malop = malop.Substring(0, malop.Length);

                                    if (!tkb[i].MaGocLHP.Equals(data[0].ToString().ToLower().Trim()))
                                        tkb[i].MaGocLHP = data[0].ToString().Trim();
                                    if (!tkb[i].MaMH.Equals(data[1].ToString().ToLower().Trim()))
                                        tkb[i].MaMH = data[1].ToString().Trim();
                                    if (!tkb[i].MaLHP.Equals(data[2].ToString().ToLower().Trim()))
                                        tkb[i].MaLHP = data[2].ToString().Trim();
                                    if (!tkb[i].TenHP.ToLower().Equals(data[3].ToString().ToLower().Trim()))
                                        tkb[i].TenHP = data[3].ToString().Trim();
                                    if (!tkb[i].SoTC.ToLower().Equals(data[4].ToString().ToLower().Trim()))
                                        tkb[i].SoTC = data[4].ToString().Trim();
                                    if (!tkb[i].LoaiHP.ToLower().Equals(data[5].ToString().ToLower().Trim()))
                                        tkb[i].LoaiHP = data[5].ToString().Trim();
                                    if (!tkb[i].MaLop.ToLower().Equals(malop.ToLower().Trim()))
                                        tkb[i].MaLop = malop;
                                    if (!tkb[i].TSMH.ToLower().Equals(data[7].ToString().ToLower().Trim()))
                                        tkb[i].TSMH = data[7].ToString().Trim();
                                    if (!tkb[i].SoTietDaXep.ToLower().Equals(data[8].ToString().ToLower().Trim()))
                                        tkb[i].SoTietDaXep = data[8].ToString().Trim();
                                    if (!tkb[i].PH.ToLower().Equals(data[9].ToString().ToLower().Trim()))
                                        tkb[i].PH = data[9].ToString().Trim();
                                    if (!tkb[i].Thu.ToLower().Equals(data[10].ToString().ToLower().Trim()))
                                        tkb[i].Thu = data[10].ToString().Trim();
                                    if (!tkb[i].TietBD.ToLower().Equals(data[11].ToString().ToLower().Trim()))
                                        tkb[i].TietBD = data[11].ToString().Trim();
                                    if (!tkb[i].SoTiet.ToLower().Equals(data[12].ToString().ToLower().Trim()))
                                        tkb[i].SoTiet = data[12].ToString().Trim();
                                    if (!tkb[i].TietHoc.ToLower().Equals(data[13].ToString().ToLower().Trim()))
                                        tkb[i].TietHoc = data[13].ToString().Trim();
                                    if (!tkb[i].Phong.ToLower().Equals(data[14].ToString().ToLower().Trim()))
                                        tkb[i].Phong = data[14].ToString().Trim();
                                    if (!tkb[i].MaCBGD.ToLower().Equals(data[15].ToString().ToLower().Trim()))
                                        tkb[i].MaCBGD = data[15].ToString().Trim();
                                    if (!tkb[i].TenCBGD.ToLower().Equals(data[16].ToString().ToLower().Trim()))
                                        tkb[i].TenCBGD = data[16].ToString().Trim();
                                    if (!tkb[i].PH_X.ToLower().Equals(data[17].ToString().ToLower().Trim()))
                                        tkb[i].PH_X = data[17].ToString().Trim();
                                    if (!tkb[i].SucChua.ToLower().Equals(data[18].ToString().ToLower().Trim()))
                                        tkb[i].SucChua = data[18].ToString().Trim();
                                    if (!tkb[i].SiSoTKB.ToLower().Equals(data[19].ToString().ToLower().Trim()))
                                        tkb[i].SiSoTKB = data[19].ToString().Trim();
                                    if (!tkb[i].Trong.ToLower().Equals(data[20].ToString().ToLower().Trim()))
                                        tkb[i].Trong = data[20].ToString().Trim();
                                    if (!tkb[i].TinhTrangLHP.ToLower().Equals(data[21].ToString().ToLower().Trim()))
                                        tkb[i].TinhTrangLHP = data[21].ToString().Trim();
                                    if (!tkb[i].TuanHoc2.ToLower().Equals(data[22].ToString().ToLower().Trim()))
                                        tkb[i].TuanHoc2 = data[22].ToString().Trim();
                                    if (!tkb[i].ThuS.ToLower().Equals(data[23].ToString().ToLower().Trim()))
                                        tkb[i].ThuS = data[23].ToString().Trim();
                                    if (!tkb[i].TietS.ToLower().Equals(data[24].ToString().ToLower().Trim()))
                                        tkb[i].TietS = data[24].ToString().Trim();
                                    if (!tkb[i].SoSVDK.ToLower().Equals(data[25].ToString().ToLower().Trim()))
                                        tkb[i].SoSVDK = data[25].ToString().Trim();
                                    if (!tkb[i].TuanBD.ToLower().Equals(data[26].ToString().ToLower().Trim()))
                                        tkb[i].TuanBD = data[26].ToString().Trim();
                                    if (!tkb[i].TuanKT.ToLower().Equals(data[27].ToString().ToLower().Trim()))
                                        tkb[i].TuanKT = data[27].ToString().Trim();
                                    if (!tkb[i].MaNganh.ToLower().Equals(data[28].ToString().ToLower().Trim()))
                                        tkb[i].MaNganh = data[28].ToString().Trim();
                                    if (!tkb[i].TenNganh.ToLower().Equals(data[29].ToString().ToLower().Trim()))
                                        tkb[i].TenNganh = data[29].ToString().Trim();
                                    if (!tkb[i].GhiChu1.ToLower().Equals(data[30].ToString().ToLower().Trim()))
                                        tkb[i].GhiChu1 = data[30].ToString().Trim();
                                    if (!tkb[i].GhiChu2.ToLower().Equals(data[31].ToString().ToLower().Trim()))
                                        tkb[i].GhiChu2 = data[31].ToString().Trim();

                                    model.Entry(tkb[i]).State = System.Data.Entity.EntityState.Modified;
                                    lstTkb.Add(tkb[i]);

                                    i++;
                                }

                                model.SaveChanges();
                            }
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

                        string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                        DataTable dt = new DataTable();
                        conString = string.Format(conString, filePath);

                        try
                        {
                            using (OleDbConnection connExcel = new OleDbConnection(conString))
                            {
                                using (OleDbCommand cmdExcel = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                    {
                                        cmdExcel.Connection = connExcel;

                                        //Get the name of First Sheet.
                                        connExcel.Open();
                                        DataTable dtSchema;
                                        dtSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                        string sheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString();
                                        connExcel.Close();

                                        //Read Data from First Sheet.
                                        connExcel.Open();
                                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                        odaExcel.SelectCommand = cmdExcel;
                                        odaExcel.Fill(dt);
                                        connExcel.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            return Json("error" + e.Message);
                        }

                        // Trim column name string
                        foreach (DataColumn col in dt.Columns)
                        {
                            col.ColumnName = col.ColumnName.Trim();
                        }

                        // Validate all columns
                        string isValid = ValidateColumns(dt);
                        if (isValid != null)
                            return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                        List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                        var lstTkb = new List<ThoiKhoaBieu>();
                        foreach (DataRow data in dt.Rows)
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

        public string ValidateColumns(DataTable dt)
        {
            // Declare the valid column names
            string[] validColumns = {
                "MaGocLHP", "Mã MH", "Mã LHP", "Tên HP", "Số TC", "Loại HP", "Mã Lớp", "TSMH",
                "Số Tiết Đã xếp", "PH", "Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "PH_X", "Sức Chứa", "SiSoTKB", "Trống", "Tình Trạng LHP", "TuanHoc2", "ThuS",
                "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Ghi Chú 1", "Ghi chú 2"
            };

            DataColumnCollection columns = dt.Columns;
            // Validate all columns in excel file
            foreach (string validColumn in validColumns)
            {
                if (!columns.Contains(validColumn))
                {
                    // Return error message
                    return validColumn;
                }
            }
            return null;
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