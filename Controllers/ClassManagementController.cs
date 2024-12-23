using QuanLyCongTacTroGiangKhoaCNTT.Models;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
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
        [Authorize, TAandGVRole]
        [HttpPost]
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
        [Authorize, GVRole]
        [HttpPost]
        public ActionResult ImportStudient(HttpPostedFileBase fileImport, int idLhp, string confirm)
        {
            try
            {
                if (fileImport != null && fileImport.ContentLength > (1024 * 1024 * 50)) // File vượt quá 50MB
                    return Content("more50mb");

                var lhp = model.LopHocPhan.Find(idLhp);

                #region --- Lần đầu import ---
                if (string.IsNullOrEmpty(confirm))
                {
                    if (lhp.DanhSachSinhVien.Count > 0)
                        return Content("Exist");

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
                    if (confirm.Equals("replace")) //Xóa rồi thêm mới
                    {
                        foreach (var item in lhp.LichHoc) //Xóa điểm danh
                        {
                            model.DiemDanh.RemoveRange(item.DiemDanh.ToList());
                            model.SaveChanges();
                        }

                        //Xóa lịch học
                        model.LichHoc.RemoveRange(lhp.LichHoc.ToList());
                        model.SaveChanges();

                        //Xóa danh sách sinh viên
                        model.DanhSachSinhVien.RemoveRange(lhp.DanhSachSinhVien.ToList());
                        model.SaveChanges();

                        //Tiến hành đọc file import
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

                        //Check số lượng cột đã đủ hay chưa
                        if (dt.Columns.Count < 9)
                            return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột, vui lòng kiểm tra lại tệp tin!");

                        //Xóa khoảng trắng các ô tiêu đề
                        foreach (DataColumn col in dt.Columns)
                            col.ColumnName = col.ColumnName.Trim();

                        //Check tên cột đã đúng hay chưa
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
                    else //cập nhật (update cái danh sách đang có sẵn)
                    {
                        var dssvs = lhp.DanhSachSinhVien.ToList();

                        //Tiến hành đọc file import
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

                        //Check số lượng cột đã đủ hay chưa
                        if (dt.Columns.Count < 9)
                            return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột, vui lòng kiểm tra lại tệp tin!");

                        //Xóa khoảng trắng các ô tiêu đề
                        foreach (DataColumn col in dt.Columns)
                            col.ColumnName = col.ColumnName.Trim();

                        //Check tên cột đã đúng hay chưa
                        string isValid = ValidateColumns(dt);
                        if (isValid != null)
                            return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                        var lstNewDSSV = new List<DanhSachSinhVien>();
                        //Đọc từng dòng dữ liệu trong file excel
                        foreach (DataRow data in dt.Rows)
                        {
                            string masv = data["Mã SV"].ToString();
                            var dssv = dssvs.FirstOrDefault(f => f.MaSV.ToLower().Equals(masv.ToLower()));
                            if (dssv != null)
                            {
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

                                dssv.HoLot = holot;
                                dssv.Ten = ten;
                                dssv.NgaySinh = Convert.ToDateTime(ngaysinh.Split('/')[2] + "-" + ngaysinh.Split('/')[1] + "-" + ngaysinh.Split('/')[0]);
                                dssv.GioiTinh = gioitinh;
                                dssv.Email = email;
                                dssv.LopHanhChinh = lophanhchinh;

                                lstNewDSSV.Add(dssv);

                            }
                        }

                        foreach (var item in lstNewDSSV)
                            model.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        model.SaveChanges();
                        model = new CongTacTroGiangKhoaCNTTEntities();

                    }
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
        /// Điểm danh lớp học.
        /// </summary>
        /// <returns>Trả về dữ liệu điểm danh lớp học.</returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult RollCall(int idLichHoc, string idsv, string trangthai, string ghichu)
        {
            try
            {
                var lhp = model.LichHoc.Find(idLichHoc);
                if (lhp == null)
                    return Content("Chi tiết lỗi: Lịch học lớp học phần không tồn tại!");

                var lstIdsv = idsv.Split('#').ToList();
                var lstTrangthai = trangthai.Split('#').ToList();
                var lstGhiChu = ghichu.Split('#').ToList();

                var lstDiemDanh = new List<DiemDanh>();
                for (int i = 0; i < lstIdsv.Count; i++)
                {
                    int idDssv = Int32.Parse(lstIdsv[i]);
                    var curDD = model.DiemDanh.FirstOrDefault(f => f.ID_LichHoc == idLichHoc && f.ID_DanhSachSinhVien == idDssv);
                    if (curDD == null)
                    {
                        DiemDanh dd = new DiemDanh();
                        dd.ID_LichHoc = idLichHoc;
                        dd.ID_DanhSachSinhVien = idDssv;
                        dd.DuLop = lstTrangthai[i];
                        dd.GhiChu = lstGhiChu[i];

                        lstDiemDanh.Add(dd);
                    }
                    else
                    {
                        curDD.DuLop = lstTrangthai[i];
                        curDD.GhiChu = lstGhiChu[i];

                        model.Entry(curDD).State = System.Data.Entity.EntityState.Modified;
                        model.SaveChanges();
                    }
                }

                if (lstDiemDanh.Count > 0)
                {
                    model.DiemDanh.AddRange(lstDiemDanh);
                    model.SaveChanges();
                }

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu và xuất file danh sách sinh viên theo LHP.
        /// </summary>
        /// <returns>Trả về danh sách sv thành file excel</returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult ExportData(int idLhp)
        {
            Session["Export-LichHoc"] = model.LichHoc.Where(w => w.ID_LopHocPhan == idLhp).ToList();
            return PartialView("_Export", model.DanhSachSinhVien.Where(w => w.ID_LopHocPhan == idLhp).ToList());
        }

        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult ResultRollCall(int idLhp)
        {
            Session["ResultRollCall"] = model.LichHoc.Where(w => w.ID_LopHocPhan == idLhp).ToList();
            return PartialView("_ResultRollCall", model.DanhSachSinhVien.Where(w => w.ID_LopHocPhan == idLhp).ToList());
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