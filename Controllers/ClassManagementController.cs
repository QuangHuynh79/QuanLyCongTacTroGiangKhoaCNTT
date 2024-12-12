using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class ClassManagementController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này trả về danh sách sinh viên của lớp học phần cho người dùng.
        /// </summary>
        /// <returns>Danh sách sinh viên của lớp học phần được chọn.</returns>
        public ActionResult ClassList(int idLhp)
        {
            var lhp = model.LopHocPhan.Find(idLhp);
            return PartialView("_ClassList", lhp.DanhSachSinhVien.ToList());
        }

        /// <summary>
        /// Hàm này trả về danh sách sinh viên của lớp học phần cho người dùng.
        /// </summary>
        /// <returns>Danh sách sinh viên của lớp học phần được chọn.</returns>
        public ActionResult ImportStudient(HttpPostedFileBase fileImport, int idLhp, string confirm)
        {
            try
            {
                if (fileImport != null && fileImport.ContentLength > (1024 * 1024 * 50)) // File vượt quá 50MB
                    return Content("more50mb");

                var lhp = model.LopHocPhan.Find(idLhp);
                if (lhp.DanhSachSinhVien.Count > 0)
                    return Content("EXIST");

                #region --- Lần đầu import ---
                if (!string.IsNullOrEmpty(confirm))
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path); //tạo đường dẫn lưu file import nếu có
                    }

                    filePath = path + Path.GetFileName(fileImport.FileName);
                    string extension = Path.GetExtension(fileImport.FileName);
                    fileImport.SaveAs(filePath);

                    string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    try
                    {
                        using (OleDbConnection connExcel = new OleDbConnection(conString)) //Đọc file excel
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

                    //Xóa khoảng trắng các ô dữ liệu
                    foreach (DataColumn col in dt.Columns)
                    {
                        col.ColumnName = col.ColumnName.Trim();
                    }

                    string isValid = ValidateColumns(dt);
                    if (isValid != null)
                        return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                    //Đọc từng dòng dữ liệu trong file excel
                    foreach (DataRow data in dt.Rows) 
                    {

                    }


                }
                #endregion

                #region Import class đã tồn tại xác nhận thay thế hay cập nhật
                else
                {

                }
                #endregion

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của các cột hợp lệ trong DataTable.
        /// </summary>
        /// <returns>Trả về tên cột thiếu nếu có, nếu không trả về null</returns>
        public string ValidateColumns(DataTable dt)
        {
            // Declare the valid column names
            string[] validColumns = {
                "Mã MH", "Mã LHP", "Tên HP"
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
    }
}