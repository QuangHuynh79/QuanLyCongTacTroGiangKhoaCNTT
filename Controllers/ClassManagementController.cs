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
        public ActionResult ClassList(int idLhp, string idLichHoc)
        {
            if (!string.IsNullOrEmpty(idLichHoc))
            {
                int idLich = Int32.Parse(idLichHoc);

                var lhp = model.LopHocPhan.Find(idLhp);
                Session["id-lhp-classlist"] = lhp;

                Session["id-current-lichhoc"] = idLich;
                return PartialView("_ClassList", lhp.DanhSachSinhVien.ToList());
            }
            else
            {
                var lhp = model.LopHocPhan.Find(idLhp);
                Session["id-lhp-classlist"] = lhp;

                var dbLichHoc = lhp.LichHoc.ToList();
                int idLich = 0;
                var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                foreach (var item in dbLichHoc.OrderBy(o => o.NgayHoc).ToList())
                {
                    if (currentDate <= item.NgayHoc)
                    {
                        idLich = item.ID;
                        break;
                    }
                }
                Session["id-current-lichhoc"] = idLich;
                return PartialView("_ClassList", lhp.DanhSachSinhVien.ToList());
            }
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
                    string path = Server.MapPath("~/Content/Uploads/ImportDanhSachSv/HK" + lhp.HocKy.TenHocKy + "/" + lhp.MaLHP + "/");
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

                    if (dt.Columns.Count < 9)
                        return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột, vui lòng kiểm tra lại tệp tin!");

                    //Xóa khoảng trắng các ô tiêu đề
                    foreach (DataColumn col in dt.Columns)
                        col.ColumnName = col.ColumnName.Trim();

                    string isValid = ValidateColumns(dt);
                    if (isValid != null)
                        return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                    var lstNewLichHoc = new List<LichHoc>();
                    for (int i = 8; i < dt.Columns.Count; i++)
                    {
                        DataColumn col = dt.Columns[i];
                        var colDate = col.ColumnName.Trim();
                        try
                        {
                            DateTime ngayHoc = Convert.ToDateTime(colDate.Split('/')[2] + "-" + colDate.Split('/')[1] + "-" + colDate.Split('/')[0]);

                            var lichHoc = new LichHoc();
                            lichHoc.ID_LopHocPhan = idLhp;
                            lichHoc.NgayHoc = ngayHoc;

                            lstNewLichHoc.Add(lichHoc);
                        }
                        catch (Exception)
                        {
                            return Content("Có vẻ như định dạng cột [" + (i + 1) + "] đã sai, phải là ngày học với định dạng [dd/mm/yyyy], vui lòng kiểm tra lại tệp tin!");
                        }

                    }

                    var lstNewDSSV = new List<DanhSachSinhVien>();
                    //Đọc từng dòng dữ liệu trong file excel
                    foreach (DataRow data in dt.Rows)
                    {
                        string masv = data["Mã SV"].ToString();
                        string holot = data["Họ lót"].ToString();
                        string ten = data["Tên"].ToString();
                        string ngaysinh = data["Ngày sinh"].ToString();
                        try
                        {
                            Convert.ToDateTime(ngaysinh.Split('/')[2] + "-" + ngaysinh.Split('/')[1] + "-" + ngaysinh.Split('/')[0]);
                        }
                        catch (Exception)
                        {
                            return Content("Có vẻ như định dạng cột [Ngày sinh] đã sai, phải là định dạng [dd/mm/yyyy], vui lòng kiểm tra lại tệp tin!");
                        }
                        string gioitinh = data["Giới tính"].ToString();
                        string email = data["Email"].ToString();
                        string lophanhchinh = data["Lớp hành chính"].ToString();

                        DanhSachSinhVien dssv = new DanhSachSinhVien();
                        dssv.ID_LopHocPhan = idLhp;
                        dssv.MaSV = masv;
                        dssv.HoLot = holot;
                        dssv.Ten = ten;
                        dssv.NgaySinh = Convert.ToDateTime(ngaysinh.Split('/')[2] + "-" + ngaysinh.Split('/')[1] + "-" + ngaysinh.Split('/')[0]);
                        dssv.GioiTinh = gioitinh;
                        dssv.Email = email;
                        dssv.LopHanhChinh = lophanhchinh;

                        lstNewDSSV.Add(dssv);
                    }

                    model.DanhSachSinhVien.AddRange(lstNewDSSV);
                    model.LichHoc.AddRange(lstNewLichHoc);
                    model.SaveChanges();
                    model = new CongTacTroGiangKhoaCNTTEntities();
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
                "Mã SV", "Họ lót", "Tên", "Ngày sinh", "Giới tính", "Email", "Lớp hành chính"
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