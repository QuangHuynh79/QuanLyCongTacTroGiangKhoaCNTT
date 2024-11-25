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
    public class TimeTableController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        // GET: TimeTable
        [Authorize, GVandBCNandTARole]
        public ActionResult Index() //Xem thời khóa biểu
        {
            return View("index");
        }

        [Authorize, GVandBCNandTARole]
        public ActionResult LoadContent() //Load dữ liệu thời khóa biểu
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 2)
                return PartialView("_IndexGV");
            else if (role == 3 || role == 5)
                return PartialView("_IndexBCN");
            else if (role == 4)
                return PartialView("_IndexTA");
            else
                return new JsonResult { Data = "SystemLoginAgain", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize, GVandBCNandTARole]
        public ActionResult FilterData(string role, int hocky, int nganh, string mon, string gv) //Lọc thời khóa biểu theo giảng viên, môn học
        {
            if (role.Equals("GV"))
            {
                var lstMon = mon.Split('#').ToList();
                var lstGv = gv.Split('#').ToList();
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH.ToLower()) && lstGv.Contains(w.LopHocPhan.MaCBGD.ToLower())).ToList();
                return PartialView("_FilterDataGV", tkb);
            }
            else
            {
                var lstMon = mon.Split('#').ToList();
                var lstGv = gv.Split('#').ToList();
                var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH.ToLower()) && lstGv.Contains(w.LopHocPhan.MaCBGD.ToLower())).ToList();
                return PartialView("_FilterData", tkb);
            }
        }
         
        [Authorize, GVandBCNandTARole]
        public ActionResult FilterParentData(string role, int hocky, int nganh) // Lọc thời khóa biểu theo học kỳ, ngành
        {
            if (role.Equals("GV"))
            {
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                string ma = taikhoan.Ma.ToLower();

                var tkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.MaCBGD.ToLower().Equals(ma)).ToList();
                return PartialView("_FilterParentDataGV", tkb);
            }
            else if(role.Equals("TA"))
            {
                var taikhoan = Session["TaiKhoan"] as TaiKhoan;
                string ma = taikhoan.Ma.ToLower();

                var tkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.LopHocPhan.UngTuyenTroGiang.Where(wl => wl.ID_TaiKhoan == taikhoan.ID).Count() > 0).ToList();
                return PartialView("_FilterParentDataGV", tkb);
            }
            else
            {
                var tkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
                return PartialView("_FilterParentData", tkb);
            }
                
        }

        [Authorize, BCNRole]
        [HttpGet]
        public ActionResult Import() //Mở view import tkb
        {
            return View("import");
        }

        [Authorize, BCNRole]
        [HttpPost] //Import thời khóa biểu
        public ActionResult SubmitImport(HttpPostedFileBase fileImport, int hocky, int nganh, string confirm)
        {
            try
            {
                if (fileImport != null && fileImport.ContentLength > (1024 * 1024 * 50)) // File vượt quá 50MB
                {
                    return Content("more50mb");
                }
                else
                {
                    var nganhdb = model.Nganh.Find(nganh);
                    if (nganhdb == null) //Ngành không tồn tại
                        return Content("NOTEXISTNGANH");

                    var hockydb = model.HocKy.Find(hocky);
                    if (hockydb == null) //Học kỳ không tồn tại
                        return Content("NOTEXISTHOCKY");
                    if (hockydb.TrangThai == false) //Trạng thái học kỳ đang đóng
                        return Content("Close");

                    if (!string.IsNullOrEmpty(confirm))
                    {
                        if (confirm.Equals("replace")) //Xóa rồi thêm mới
                        {
                            var currentTkb = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.ThoiKhoaBieu.RemoveRange(currentTkb); //Xóa tkb cũ
                            model.SaveChanges();

                            var currentHocPhan = model.LopHocPhan.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.LopHocPhan.RemoveRange(currentHocPhan); //Xóa lớp học phần của tkb cũ
                            model.SaveChanges();

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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            foreach (DataRow data in dt.Rows) //Đọc từng dòng dữ liệu trong file excel
                            {
                                if (!string.IsNullOrEmpty(data["Mã Ngành"].ToString()) && !string.IsNullOrEmpty(data["Tên Ngành"].ToString()))
                                    if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                        && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                        continue;

                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string type = data["Loại HP"].ToString();
                                string totalLesson = data["Số Tiết Đã xếp"].ToString();
                                string day = data["Thứ"].ToString();
                                string startLesson = data["Tiết BĐ"].ToString();
                                string lessonNumber = data["Số Tiết"].ToString();
                                string lessonTime = data["Tiết Học"].ToString();
                                string roomId = data["Phòng"].ToString();
                                string lecturerId = data["Mã CBGD"].ToString();
                                string fullName = data["Tên CBGD"].ToString();
                                string day2 = data["ThuS"].ToString();
                                string startLesson2 = data["TietS"].ToString();
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();
                                string endWeek = data["Tuần KT"].ToString();
                                string idmajor = data["Mã Ngành"].ToString();
                                string namemajor = data["Tên Ngành"].ToString();

                                // Check dữ liệu trống
                                string[] validRows = { subjectId, classSectionid, name, type, totalLesson, day, startLesson
                                        , lessonNumber, lessonTime, roomId, lecturerId, fullName
                                    , day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
                                string checkNull = ValidateNotNull(validRows);
                                if (checkNull != null)
                                {
                                    int excelRow = dt.Rows.IndexOf(data) + 2;
                                    return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], Có trường dữ liệu chưa được nạp vui lòng kiểm tra lại tệp tin.");
                                }

                                if (!new List<int> { 1, 4, 7, 10, 13 }.Contains(Int32.Parse(startLesson2.Trim())))
                                {
                                    int excelRow = dt.Rows.IndexOf(data) + 2;
                                    return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], tiết bắt đầu phải là 1, 4, 7, 10 hoặc 13.");
                                }
                            }
                            foreach (DataRow data in dt.Rows) //Đọc từng dòng dữ liệu trong file excel
                            {
                                if (!string.IsNullOrEmpty(data["Mã Ngành"].ToString()) && !string.IsNullOrEmpty(data["Tên Ngành"].ToString()))
                                    if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                        && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                        continue;

                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string type = data["Loại HP"].ToString();
                                string totalLesson = data["Số Tiết Đã xếp"].ToString();
                                string day = data["Thứ"].ToString();
                                string startLesson = data["Tiết BĐ"].ToString();
                                string lessonNumber = data["Số Tiết"].ToString();
                                string lessonTime = data["Tiết Học"].ToString();
                                string roomId = data["Phòng"].ToString();
                                string lecturerId = data["Mã CBGD"].ToString();
                                string fullName = data["Tên CBGD"].ToString();
                                string day2 = data["ThuS"].ToString();
                                string startLesson2 = data["TietS"].ToString();
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();
                                string endWeek = data["Tuần KT"].ToString();
                                string idmajor = data["Mã Ngành"].ToString();
                                string namemajor = data["Tên Ngành"].ToString();

                                model = new CongTacTroGiangKhoaCNTTEntities();
                                var hocphanExist = model.LopHocPhan.FirstOrDefault(w => w.ID_HocKy == hocky
                                && w.ID_Nganh == nganh
                                && w.MaLHP.ToLower().Equals(classSectionid)); //Kiểm tra lhp đã tồn tại chưa

                                int idHp = 0;
                                if (hocphanExist != null) //Đã tồn tại thì chỉ lấy ID để gán cho tkb thôi
                                {
                                    idHp = hocphanExist.ID;
                                }
                                else //Chưa tồn tại thì tạo mới lhp
                                {
                                    var hocphan = new LopHocPhan()
                                    {
                                        ID_HocKy = hocky,
                                        ID_Nganh = nganh,
                                        MaMH = subjectId,
                                        MaLHP = classSectionid,
                                        TenHP = name,
                                        LoaiHP = type,
                                        MaCBGD = lecturerId,
                                        TenCBGD = fullName,
                                    };

                                    var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                                    if (tkGv != null)
                                        hocphan.ID_TaiKhoan = tkGv.ID;

                                    model.LopHocPhan.Add(hocphan);
                                    model.SaveChanges();
                                    idHp = hocphan.ID;
                                }

                                var tkb = new ThoiKhoaBieu(); //Tạo thời khóa biểu
                                tkb.ID_HocKy = hocky;
                                tkb.ID_Nganh = nganh;
                                tkb.ID_LopHocPhan = idHp;
                                tkb.SoTietDaXep = Int32.Parse(totalLesson);
                                tkb.Thu = day;
                                tkb.TietBD = Int32.Parse(startLesson);
                                tkb.SoTiet = Int32.Parse(lessonNumber);
                                tkb.TietHoc = lessonTime;
                                tkb.ThuS = Int32.Parse(day2);
                                tkb.TietS = Int32.Parse(startLesson2);
                                tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                                tkb.TuanBD = Int32.Parse(startWeek);
                                tkb.TuanKT = Int32.Parse(endWeek);
                                tkb.Phong = roomId;

                                model.ThoiKhoaBieu.Add(tkb);
                                model.SaveChanges();

                            }
                        }
                        else //cập nhật (update cái tkb đang có sẵn)
                        {
                            var tkbs = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList(); //Lấy ra danh sách tkb cần update

                            string filePath = string.Empty;
                            string path = Server.MapPath("~/Content/Uploads/ImportTKB/"); //Lấy đường dẫn đã lưu file import
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path); //Tạo folder chứa file nếu chưa có
                            }

                            filePath = path + Path.GetFileName(fileImport.FileName);
                            string extension = Path.GetExtension(fileImport.FileName);
                            fileImport.SaveAs(filePath); //Lưu file import

                            string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                            DataTable dt = new DataTable();
                            conString = string.Format(conString, filePath); //Lấy cấu hình đọc file Excel trong file config

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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            foreach (DataRow data in dt.Rows) //Đọc từng dòng trong file excel
                            {
                                var tkb = tkbs.FirstOrDefault(f => f.LopHocPhan.MaLHP.Equals(data["Mã LHP"].ToString()));
                                if (tkb == null)
                                    continue;

                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string type = data["Loại HP"].ToString();
                                string totalLesson = data["Số Tiết Đã xếp"].ToString();
                                string day = data["Thứ"].ToString();
                                string startLesson = data["Tiết BĐ"].ToString();
                                string lessonNumber = data["Số Tiết"].ToString();
                                string lessonTime = data["Tiết Học"].ToString();
                                string roomId = data["Phòng"].ToString();
                                string lecturerId = data["Mã CBGD"].ToString();
                                string fullName = data["Tên CBGD"].ToString();
                                string day2 = data["ThuS"].ToString();
                                string startLesson2 = data["TietS"].ToString();
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();
                                string endWeek = data["Tuần KT"].ToString();
                                string idmajor = data["Mã Ngành"].ToString();
                                string namemajor = data["Tên Ngành"].ToString();

                                // Check dữ liệu bị bỏ trống
                                string[] validRows = { subjectId, classSectionid, name, type, totalLesson, day, startLesson
                                        , lessonNumber, lessonTime, roomId, lecturerId, fullName
                                    , day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
                                string checkNull = ValidateNotNull(validRows);
                                if (checkNull != null)
                                {
                                    int excelRow = dt.Rows.IndexOf(data) + 2;
                                    return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], Có trường dữ liệu chưa được nạp vui lòng kiểm tra lại tệp tin.");
                                }

                                if (!new List<int> { 1, 4, 7, 10, 13 }.Contains(Int32.Parse(startLesson2.Trim())))
                                {
                                    int excelRow = dt.Rows.IndexOf(data) + 2;
                                    return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], tiết bắt đầu phải là 1, 4, 7, 10 hoặc 13.");
                                }
                            }

                            foreach (DataRow data in dt.Rows) //Đọc từng dòng trong file excel
                            {
                                var tkb = tkbs.FirstOrDefault(f => f.LopHocPhan.MaLHP.Equals(data["Mã LHP"].ToString()));
                                if (tkb == null)
                                    continue;

                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string type = data["Loại HP"].ToString();
                                string totalLesson = data["Số Tiết Đã xếp"].ToString();
                                string day = data["Thứ"].ToString();
                                string startLesson = data["Tiết BĐ"].ToString();
                                string lessonNumber = data["Số Tiết"].ToString();
                                string lessonTime = data["Tiết Học"].ToString();
                                string roomId = data["Phòng"].ToString();
                                string lecturerId = data["Mã CBGD"].ToString();
                                string fullName = data["Tên CBGD"].ToString();
                                string day2 = data["ThuS"].ToString();
                                string startLesson2 = data["TietS"].ToString();
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();
                                string endWeek = data["Tuần KT"].ToString();
                                string idmajor = data["Mã Ngành"].ToString();
                                string namemajor = data["Tên Ngành"].ToString();

                                //Cập nhật thông tin về lớp học phần
                                var hocphan = tkb.LopHocPhan;
                                hocphan.MaMH = subjectId;
                                hocphan.TenHP = name;
                                hocphan.LoaiHP = type;
                                hocphan.MaCBGD = lecturerId;
                                hocphan.TenCBGD = fullName;

                                var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                                if (tkGv != null)
                                    hocphan.ID_TaiKhoan = tkGv.ID;

                                //Cập nhật thông tin về thời khóa biểu
                                tkb.SoTietDaXep = Int32.Parse(totalLesson);
                                tkb.Thu = day;
                                tkb.TietBD = Int32.Parse(startLesson);
                                tkb.SoTiet = Int32.Parse(lessonNumber);
                                tkb.TietHoc = lessonTime;
                                tkb.ThuS = Int32.Parse(day2);
                                tkb.TietS = Int32.Parse(startLesson2);
                                tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                                tkb.TuanBD = Int32.Parse(startWeek);
                                tkb.TuanKT = Int32.Parse(endWeek);
                                tkb.Phong = roomId;

                                model.Entry(hocphan).State = System.Data.Entity.EntityState.Modified;
                                model.Entry(tkb).State = System.Data.Entity.EntityState.Modified;
                                model.SaveChanges();
                            }
                        }
                    }
                    else //Lần đầu import
                    {
                        var checkExist = model.ThoiKhoaBieu.FirstOrDefault(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh);
                        if (checkExist != null) //check đã tồn tại hay chưa
                            return Content("Exist");

                        string filePath = string.Empty;
                        string path = Server.MapPath("~/Content/Uploads/ImportTKB/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path); //tạo folder lưu file import
                        }

                        filePath = path + Path.GetFileName(fileImport.FileName);
                        string extension = Path.GetExtension(fileImport.FileName);
                        fileImport.SaveAs(filePath);

                        string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;
                        DataTable dt = new DataTable();
                        conString = string.Format(conString, filePath);

                        try
                        {
                            using (OleDbConnection connExcel = new OleDbConnection(conString)) //Đọc file import
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

                        foreach (DataColumn col in dt.Columns)
                        {
                            col.ColumnName = col.ColumnName.Trim();
                        }

                        string isValid = ValidateColumns(dt);
                        if (isValid != null)
                            return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                        foreach (DataRow data in dt.Rows) //Đọc từng dòng trong file import
                        {
                            if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                continue;

                            string subjectId = data["Mã MH"].ToString();
                            string classSectionid = data["Mã LHP"].ToString();
                            string name = data["Tên HP"].ToString();
                            string type = data["Loại HP"].ToString();
                            string totalLesson = data["Số Tiết Đã xếp"].ToString();
                            string day = data["Thứ"].ToString();
                            string startLesson = data["Tiết BĐ"].ToString();
                            string lessonNumber = data["Số Tiết"].ToString();
                            string lessonTime = data["Tiết Học"].ToString();
                            string roomId = data["Phòng"].ToString();
                            string lecturerId = data["Mã CBGD"].ToString();
                            string fullName = data["Tên CBGD"].ToString();
                            string day2 = data["ThuS"].ToString();
                            string startLesson2 = data["TietS"].ToString();
                            string studentRegisteredNumber = data["Số SVĐK"].ToString();
                            string startWeek = data["Tuần BD"].ToString();
                            string endWeek = data["Tuần KT"].ToString();
                            string idmajor = data["Mã Ngành"].ToString();
                            string namemajor = data["Tên Ngành"].ToString();

                            // Check if values is null
                            string[] validRows = { subjectId, classSectionid, name, type, totalLesson, day, startLesson
                                        , lessonNumber, lessonTime, roomId, lecturerId, fullName
                                    , day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
                            string checkNull = ValidateNotNull(validRows);
                            if (checkNull != null)
                            {
                                int excelRow = dt.Rows.IndexOf(data) + 2;
                                return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], Có trường dữ liệu chưa được nạp vui lòng kiểm tra lại tệp tin.");
                            }

                            if (!new List<int> { 1, 4, 7, 10, 13 }.Contains(Int32.Parse(startLesson2.Trim())))
                            {
                                int excelRow = dt.Rows.IndexOf(data) + 2;
                                return Content("Đã có lỗi đã xảy ra ở dòng số [" + excelRow + "], tiết bắt đầu phải là 1, 4, 7, 10 hoặc 13.");
                            }
                        }

                        foreach (DataRow data in dt.Rows) //Đọc từng dòng trong file import
                        {
                            if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                continue;

                            string subjectId = data["Mã MH"].ToString();
                            string classSectionid = data["Mã LHP"].ToString();
                            string name = data["Tên HP"].ToString();
                            string type = data["Loại HP"].ToString();
                            string totalLesson = data["Số Tiết Đã xếp"].ToString();
                            string day = data["Thứ"].ToString();
                            string startLesson = data["Tiết BĐ"].ToString();
                            string lessonNumber = data["Số Tiết"].ToString();
                            string lessonTime = data["Tiết Học"].ToString();
                            string roomId = data["Phòng"].ToString();
                            string lecturerId = data["Mã CBGD"].ToString();
                            string fullName = data["Tên CBGD"].ToString();
                            string day2 = data["ThuS"].ToString();
                            string startLesson2 = data["TietS"].ToString();
                            string studentRegisteredNumber = data["Số SVĐK"].ToString();
                            string startWeek = data["Tuần BD"].ToString();
                            string endWeek = data["Tuần KT"].ToString();
                            string idmajor = data["Mã Ngành"].ToString();
                            string namemajor = data["Tên Ngành"].ToString();

                            model = new CongTacTroGiangKhoaCNTTEntities();

                            //Check lhp đã tồn tại hay chưa
                            var hocphanExist = model.LopHocPhan.FirstOrDefault(w => w.ID_HocKy == hocky
                            && w.ID_Nganh == nganh
                            && w.MaLHP.ToLower().Equals(classSectionid));

                            int idHp = 0;
                            if (hocphanExist != null) //Đã tồn tại thì chỉ lấy id để gán vào tkb thôi
                            {
                                idHp = hocphanExist.ID;
                            }
                            else
                            {
                                var hocphan = new LopHocPhan() //Tạo mới lhp
                                {
                                    ID_HocKy = hocky,
                                    ID_Nganh = nganh,
                                    MaMH = subjectId,
                                    MaLHP = classSectionid,
                                    TenHP = name,
                                    LoaiHP = type,
                                    MaCBGD = lecturerId,
                                    TenCBGD = fullName,
                                };

                                var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                                if (tkGv != null)
                                    hocphan.ID_TaiKhoan = tkGv.ID;

                                model.LopHocPhan.Add(hocphan);
                                model.SaveChanges();
                                idHp = hocphan.ID;
                            }

                            var tkb = new ThoiKhoaBieu(); //tạo thời khóa biểu
                            tkb.ID_HocKy = hocky;
                            tkb.ID_Nganh = nganh;
                            tkb.ID_LopHocPhan = idHp;
                            tkb.SoTietDaXep = Int32.Parse(totalLesson);
                            tkb.Thu = day;
                            tkb.TietBD = Int32.Parse(startLesson);
                            tkb.SoTiet = Int32.Parse(lessonNumber);
                            tkb.TietHoc = lessonTime;
                            tkb.ThuS = Int32.Parse(day2);
                            tkb.TietS = Int32.Parse(startLesson2);
                            tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                            tkb.TuanBD = Int32.Parse(startWeek);
                            tkb.TuanKT = Int32.Parse(endWeek);
                            tkb.Phong = roomId;

                            model.ThoiKhoaBieu.Add(tkb);
                            model.SaveChanges();

                        }
                    }
                    return Content("SUCCESS");
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ContentResult DownloadFile(string fileName)
        {
            fileName = "CNTT UIS-ThoiKhoaBieu_TieuChuan_Mau.xlsx";

            string path = Server.MapPath("~/Content/timetable/formimport/");
            byte[] bytes = System.IO.File.ReadAllBytes(path + fileName);
            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

            return Content(base64);
        }

        [Authorize, BCNRole]
        [HttpPost]
        public ActionResult ExportTimeTable(int hocky, int nganh, string mon, string gv)
        {

            var lstMon = mon.Split('#').ToList();
            var lstGv = gv.Split('#').ToList();
            var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
            var tkb = lstTkb.Where(w => lstMon.Contains(w.LopHocPhan.MaMH) && lstGv.Contains(w.LopHocPhan.MaCBGD)).ToList();

            return PartialView("_Export", tkb);
        }

        public string ValidateColumns(DataTable dt)
        {
            // Declare the valid column names
            string[] validColumns = {
                "Mã MH", "Mã LHP", "Tên HP", "Loại HP",
                "Số Tiết Đã xếp","Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "ThuS", "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Mã Ngành", "Tên Ngành"
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

        public string ValidateNotNull(string[] validRows)
        {
            foreach (string validRow in validRows)
            {
                // Check if string is null
                if (string.IsNullOrEmpty(validRow))
                {
                    return validRow;
                }
            }
            return null;
        }

    }
}