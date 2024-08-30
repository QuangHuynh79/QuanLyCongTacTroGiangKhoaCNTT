using Microsoft.AspNet.Identity;
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
        // GET: studentaffairs/timetable
        public ActionResult Index()
        {
            return View("index");
        }

        [HttpGet]
        public ActionResult Import()
        {
            return View("import");
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase fileImport)
        {
            try
            {
                if (fileImport != null && fileImport.ContentLength > (1024 * 1024 * 50)) // 50MB limit
                {
                    return Content("File Import", "Vui lòng import file có kích thước nhỏ hơn 50MB");
                }
                else
                {
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

                    if (!dr[1].ToString().Trim().Equals("MaGocLHP")
                        || !dr[2].ToString().Trim().Equals("Mã MH")
                        || !dr[3].ToString().Trim().Equals("Mã LHP")
                        || !dr[4].ToString().Trim().Equals("Tên HP")
                        || !dr[5].ToString().Trim().Equals("Số SVĐK")
                        || !dr[6].ToString().Trim().Equals("MaGocLHP")
                        || !dr[7].ToString().Trim().Equals("MaGocLHP")
                        || !dr[8].ToString().Trim().Equals("MaGocLHP")
                        || !dr[9].ToString().Trim().Equals("MaGocLHP")
                        || !dr[10].ToString().Trim().Equals("MaGocLHP")
                        || !dr[11].ToString().Trim().Equals("MaGocLHP")
                        || !dr[12].ToString().Trim().Equals("MaGocLHP")
                        || !dr[13].ToString().Trim().Equals("MaGocLHP")
                        || !dr[14].ToString().Trim().Equals("MaGocLHP")
                        || !dr[15].ToString().Trim().Equals("MaGocLHP")
                        || !dr[16].ToString().Trim().Equals("MaGocLHP")
                        || !dr[17].ToString().Trim().Equals("MaGocLHP")
                        || !dr[18].ToString().Trim().Equals("MaGocLHP")
                        || !dr[19].ToString().Trim().Equals("MaGocLHP")
                        || !dr[20].ToString().Trim().Equals("MaGocLHP")
                        || !dr[21].ToString().Trim().Equals("MaGocLHP")
                        || !dr[22].ToString().Trim().Equals("MaGocLHP")
                        || !dr[23].ToString().Trim().Equals("MaGocLHP")
                        || !dr[24].ToString().Trim().Equals("MaGocLHP")
                        || !dr[25].ToString().Trim().Equals("MaGocLHP")
                        || !dr[26].ToString().Trim().Equals("MaGocLHP")
                        || !dr[27].ToString().Trim().Equals("MaGocLHP")
                        || !dr[28].ToString().Trim().Equals("MaGocLHP")
                        || !dr[29].ToString().Trim().Equals("MaGocLHP")
                        || !dr[30].ToString().Trim().Equals("MaGocLHP")
                        || !dr[31].ToString().Trim().Equals("MaGocLHP")
                        || !dr[32].ToString().Trim().Equals("MaGocLHP"))
                    {
                        return Content("INCORRECT");
                    }
                    return Content("SUCCESS");
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}