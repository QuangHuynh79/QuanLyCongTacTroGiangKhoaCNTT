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
        public ActionResult Index()
        {
            return View("index");
        }

        [Authorize, GVandBCNandTARole]
        public ActionResult LoadContent()
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 2)
                return PartialView("_IndexGV");
            else if (role == 3)
                return PartialView("_IndexBCN");
            else if (role == 4)
                return PartialView("_IndexTA");
            else
                return new JsonResult { Data = "SystemLoginAgain", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize, BCNRole]
        public ActionResult FilterData(int hocky, int nganh, string mon, string gv)
        {
            var lstMon = mon.Split('#').ToList();
            var lstGv = gv.Split('#').ToList();
            var lstTkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
            var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
            return PartialView("_FilterData", tkb);
        }


        [Authorize, BCNRole]
        public ActionResult FilterParentData(int hocky, int nganh, string mon, string gv)
        {
            var tkb = model.ThoiKhoaBieu.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh).ToList();
            return PartialView("_FilterParentData", tkb);
        }

        [Authorize, BCNRole]
        [HttpGet]
        public ActionResult Import()
        {
            return View("import");
        }

        [Authorize, BCNRole]
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
                    var nganhdb = model.Nganh.Find(nganh);
                    if (nganhdb == null)
                        return Content("NOTEXISTNGANH");

                    var hockydb = model.HocKy.Find(hocky);
                    if (hockydb == null)
                        return Content("NOTEXISTHOCKY");
                    if (hockydb.TrangThai == false)
                        return Content("Close");

                    if (!string.IsNullOrEmpty(confirm))
                    {
                        if (confirm.Equals("replace")) //Xóa rồi thêm mới
                        {
                            var currentTkb = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.ThoiKhoaBieu.RemoveRange(currentTkb);
                            model.SaveChanges();

                            var currentHocPhan = model.HocPhan.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.HocPhan.RemoveRange(currentHocPhan);
                            model.SaveChanges();

                            var currentPhong = model.Phong.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();
                            model.Phong.RemoveRange(currentPhong);
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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                            foreach (DataRow data in dt.Rows)
                            {
                                if (!string.IsNullOrEmpty(data["Mã Ngành"].ToString()) && !string.IsNullOrEmpty(data["Tên Ngành"].ToString()))
                                    if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                        && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                        continue;

                                string originalId = data["MaGocLHP"].ToString();
                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string credits = data["Số TC"].ToString();
                                string type = data["Loại HP"].ToString();
                                string studentClassId = data["Mã Lớp"].ToString();
                                var malop = "";
                                foreach (var item in data["Mã Lớp"].ToString().Split('\n'))
                                {
                                    malop += item + "#";
                                }
                                malop = malop.Substring(0, malop.Length - 1);
                                string minimumStudent = data["TSMH"].ToString(); //
                                string totalLesson = data["Số Tiết Đã xếp"].ToString(); //
                                string room2 = data["PH"].ToString();
                                string day = data["Thứ"].ToString(); //
                                string startLesson = data["Tiết BĐ"].ToString(); //
                                string lessonNumber = data["Số Tiết"].ToString();//
                                string lessonTime = data["Tiết Học"].ToString();//
                                string roomId = data["Phòng"].ToString();//
                                string lecturerId = data["Mã CBGD"].ToString();//
                                string fullName = data["Tên CBGD"].ToString();//
                                string roomType = data["PH_X"].ToString();//
                                string capacity = data["Sức Chứa"].ToString();
                                string studentNumber = data["SiSoTKB"].ToString();//
                                string freeSlot = data["Trống"].ToString();//
                                string state = data["Tình Trạng LHP"].ToString();//
                                string learnWeek = data["TuanHoc2"].ToString();//
                                string day2 = data["ThuS"].ToString();//
                                string startLesson2 = data["TietS"].ToString();//
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();//
                                string endWeek = data["Tuần KT"].ToString();//
                                string idmajor = data["Mã Ngành"].ToString();//
                                string namemajor = data["Tên Ngành"].ToString();//
                                string note1 = data["Ghi Chú 1"].ToString();
                                string note2 = data["Ghi chú 2"].ToString();

                                // Check if values is null
                                string[] validRows = { originalId, subjectId, classSectionid, name, credits, type, studentClassId, minimumStudent
                                    , totalLesson, day, startLesson, lessonNumber, lessonTime, roomId, lecturerId, fullName, roomType, studentNumber
                                    , freeSlot, state, learnWeek, day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
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

                                var hocphan = new HocPhan()
                                {
                                    ID_HocKy = hocky,
                                    ID_Nganh = nganh,
                                    MaMH = subjectId,
                                    TenHP = name,
                                    SoTC = Int32.Parse(credits),
                                };
                                model.HocPhan.Add(hocphan);
                                model.SaveChanges();
                                int idHp = hocphan.ID;

                                var phong = new Phong()
                                {
                                    ID_HocKy = hocky,
                                    ID_Nganh = nganh,
                                    TenPhong = roomId,
                                    PH = room2,
                                    PH_X = roomType,
                                    SucChua = Int32.Parse(capacity),
                                };

                                model.Phong.Add(phong);
                                model.SaveChanges();
                                int idPhong = phong.ID;

                                var tkb = new ThoiKhoaBieu();
                                tkb.ID_HocKy = hocky;
                                tkb.ID_Nganh = nganh;
                                tkb.ID_HocPhan = idHp;
                                tkb.ID_Phong = idPhong;
                                tkb.MaGocLHP = originalId;
                                tkb.MaLHP = classSectionid;
                                tkb.MaMH = subjectId;
                                tkb.LoaiHP = type;
                                tkb.MaLop = malop;
                                tkb.TSMH = Int32.Parse(minimumStudent);
                                tkb.SoTietDaXep = Int32.Parse(totalLesson);
                                tkb.Thu = day;
                                tkb.TietBD = Int32.Parse(startLesson);
                                tkb.SoTiet = Int32.Parse(lessonNumber);
                                tkb.TietHoc = lessonTime;
                                tkb.MaCBGD = lecturerId;
                                tkb.TenCBGD = fullName;
                                tkb.SiSoTKB = Int32.Parse(studentNumber);
                                tkb.Trong = Int32.Parse(freeSlot);
                                tkb.TinhTrangLHP = state;
                                tkb.TuanHoc2 = learnWeek;
                                tkb.ThuS = Int32.Parse(day2);
                                tkb.TietS = Int32.Parse(startLesson2);
                                tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                                tkb.TuanBD = Int32.Parse(startWeek);
                                tkb.TuanKT = Int32.Parse(endWeek);
                                tkb.GhiChu1 = note1;
                                tkb.GhiChu2 = note2;

                                var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                                if (tkGv != null)
                                    tkb.ID_TaiKhoan = tkGv.ID;

                                model.ThoiKhoaBieu.Add(tkb);
                                model.SaveChanges();
                            }
                        }
                        else //cập nhật (update cái có sẵn)
                        {
                            var tkbs = model.ThoiKhoaBieu.Where(t => t.ID_HocKy == hocky && t.ID_Nganh == nganh).ToList();

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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            foreach (DataRow data in dt.Rows)
                            {
                                var tkb = tkbs.FirstOrDefault(f => f.MaLHP.Equals(data["Mã LHP"].ToString()));
                                if (tkb == null)
                                    continue;

                                string originalId = data["MaGocLHP"].ToString();
                                string subjectId = data["Mã MH"].ToString();
                                string classSectionid = data["Mã LHP"].ToString();
                                string name = data["Tên HP"].ToString();
                                string credits = data["Số TC"].ToString();
                                string type = data["Loại HP"].ToString();
                                string studentClassId = data["Mã Lớp"].ToString();
                                var malop = "";
                                foreach (var item in data["Mã Lớp"].ToString().Split('\n'))
                                {
                                    malop += item + "#";
                                }
                                malop = malop.Substring(0, malop.Length - 1);
                                string minimumStudent = data["TSMH"].ToString(); //
                                string totalLesson = data["Số Tiết Đã xếp"].ToString(); //
                                string room2 = data["PH"].ToString();
                                string day = data["Thứ"].ToString(); //
                                string startLesson = data["Tiết BĐ"].ToString(); //
                                string lessonNumber = data["Số Tiết"].ToString();//
                                string lessonTime = data["Tiết Học"].ToString();//
                                string roomId = data["Phòng"].ToString();//
                                string lecturerId = data["Mã CBGD"].ToString();//
                                string fullName = data["Tên CBGD"].ToString();//
                                string roomType = data["PH_X"].ToString();//
                                string capacity = data["Sức Chứa"].ToString();
                                string studentNumber = data["SiSoTKB"].ToString();//
                                string freeSlot = data["Trống"].ToString();//
                                string state = data["Tình Trạng LHP"].ToString();//
                                string learnWeek = data["TuanHoc2"].ToString();//
                                string day2 = data["ThuS"].ToString();//
                                string startLesson2 = data["TietS"].ToString();//
                                string studentRegisteredNumber = data["Số SVĐK"].ToString();
                                string startWeek = data["Tuần BD"].ToString();//
                                string endWeek = data["Tuần KT"].ToString();//
                                string idmajor = data["Mã Ngành"].ToString();//
                                string namemajor = data["Tên Ngành"].ToString();//
                                string note1 = data["Ghi Chú 1"].ToString();
                                string note2 = data["Ghi chú 2"].ToString();

                                // Check if values is null
                                string[] validRows = { originalId, subjectId, classSectionid, name, credits, type, studentClassId, minimumStudent
                                    , totalLesson, day, startLesson, lessonNumber, lessonTime, roomId, lecturerId, fullName, roomType, studentNumber
                                    , freeSlot, state, learnWeek, day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
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

                                var hocphan = tkb.HocPhan;
                                hocphan.ID_HocKy = hocky;
                                hocphan.ID_Nganh = nganh;
                                hocphan.MaMH = subjectId;
                                hocphan.TenHP = name;
                                hocphan.SoTC = Int32.Parse(credits);

                                model.Entry(hocphan).State = System.Data.Entity.EntityState.Modified;
                                model.SaveChanges();

                                var phong = tkb.Phong;
                                phong.ID_HocKy = hocky;
                                phong.ID_Nganh = nganh;
                                phong.TenPhong = roomId;
                                phong.PH = room2;
                                phong.PH_X = roomType;
                                phong.SucChua = Int32.Parse(capacity);

                                model.Entry(phong).State = System.Data.Entity.EntityState.Modified;
                                model.SaveChanges();

                                tkb.MaGocLHP = originalId;
                                tkb.MaMH = subjectId;
                                tkb.LoaiHP = type;
                                tkb.MaLop = malop;
                                tkb.TSMH = Int32.Parse(minimumStudent);
                                tkb.SoTietDaXep = Int32.Parse(totalLesson);
                                tkb.Thu = day;
                                tkb.TietBD = Int32.Parse(startLesson);
                                tkb.SoTiet = Int32.Parse(lessonNumber);
                                tkb.TietHoc = lessonTime;
                                tkb.MaCBGD = lecturerId;
                                tkb.TenCBGD = fullName;
                                tkb.SiSoTKB = Int32.Parse(studentNumber);
                                tkb.Trong = Int32.Parse(freeSlot);
                                tkb.TinhTrangLHP = state;
                                tkb.TuanHoc2 = learnWeek;
                                tkb.ThuS = Int32.Parse(day2);
                                tkb.TietS = Int32.Parse(startLesson2);
                                tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                                tkb.TuanBD = Int32.Parse(startWeek);
                                tkb.TuanKT = Int32.Parse(endWeek);
                                tkb.GhiChu1 = note1;
                                tkb.GhiChu2 = note2;

                                var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                                if (tkGv != null)
                                    tkb.ID_TaiKhoan = tkGv.ID;

                                model.Entry(tkb).State = System.Data.Entity.EntityState.Modified;
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

                        foreach (DataColumn col in dt.Columns)
                        {
                            col.ColumnName = col.ColumnName.Trim();
                        }

                        string isValid = ValidateColumns(dt);
                        if (isValid != null)
                            return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                        List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                        foreach (DataRow data in dt.Rows)
                        {
                            if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                continue;

                            string originalId = data["MaGocLHP"].ToString(); //
                            string subjectId = data["Mã MH"].ToString();////
                            string classSectionid = data["Mã LHP"].ToString(); //
                            string name = data["Tên HP"].ToString();  ////
                            string credits = data["Số TC"].ToString(); ////
                            string type = data["Loại HP"].ToString(); //

                            string studentClassId = data["Mã Lớp"].ToString(); //
                            var malop = "";
                            foreach (var item in data["Mã Lớp"].ToString().Split('\n'))
                            {
                                malop += item + "#";
                            }
                            malop = malop.Substring(0, malop.Length - 1);

                            string minimumStudent = data["TSMH"].ToString(); //
                            string totalLesson = data["Số Tiết Đã xếp"].ToString(); //
                            string day = data["Thứ"].ToString(); //
                            string startLesson = data["Tiết BĐ"].ToString(); //
                            string lessonNumber = data["Số Tiết"].ToString(); //
                            string lessonTime = data["Tiết Học"].ToString(); //
                            string room2 = data["PH"].ToString();
                            string roomId = data["Phòng"].ToString();
                            string lecturerId = data["Mã CBGD"].ToString();//
                            string fullName = data["Tên CBGD"].ToString();//
                            string roomType = data["PH_X"].ToString();
                            string capacity = data["Sức Chứa"].ToString();
                            string studentNumber = data["SiSoTKB"].ToString(); //
                            string freeSlot = data["Trống"].ToString(); //
                            string state = data["Tình Trạng LHP"].ToString(); //
                            string learnWeek = data["TuanHoc2"].ToString(); //
                            string day2 = data["ThuS"].ToString(); //
                            string startLesson2 = data["TietS"].ToString(); //
                            string studentRegisteredNumber = data["Số SVĐK"].ToString(); //
                            string startWeek = data["Tuần BD"].ToString(); //
                            string endWeek = data["Tuần KT"].ToString(); //
                            string idmajor = data["Mã Ngành"].ToString();
                            string namemajor = data["Tên Ngành"].ToString();
                            string note1 = data["Ghi Chú 1"].ToString(); //
                            string note2 = data["Ghi chú 2"].ToString(); //

                            // Check if values is null
                            string[] validRows = { originalId, subjectId, classSectionid, name, credits, type, studentClassId, minimumStudent
                                    , totalLesson, day, startLesson, lessonNumber, lessonTime, roomId, lecturerId, fullName, roomType, studentNumber
                                    , freeSlot, state, learnWeek, day2, startLesson2, idmajor, namemajor, startWeek, endWeek };
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

                            var hocphan = new HocPhan()
                            {
                                ID_HocKy = hocky,
                                ID_Nganh = nganh,
                                MaMH = subjectId,
                                TenHP = name,
                                SoTC = Int32.Parse(credits),
                            };
                            model.HocPhan.Add(hocphan);
                            model.SaveChanges();
                            int idHp = hocphan.ID;

                            var phong = new Phong()
                            {
                                ID_HocKy = hocky,
                                ID_Nganh = nganh,
                                TenPhong = roomId,
                                PH = room2,
                                PH_X = roomType,
                                SucChua = Int32.Parse(capacity),
                            };

                            model.Phong.Add(phong);
                            model.SaveChanges();
                            int idPhong = phong.ID;

                            var tkb = new ThoiKhoaBieu();
                            tkb.ID_HocKy = hocky;
                            tkb.ID_Nganh = nganh;
                            tkb.ID_HocPhan = idHp;
                            tkb.ID_Phong = idPhong;
                            tkb.MaGocLHP = originalId;
                            tkb.MaLHP = classSectionid;
                            tkb.MaMH = subjectId;
                            tkb.LoaiHP = type;
                            tkb.MaLop = malop;
                            tkb.TSMH = Int32.Parse(minimumStudent);
                            tkb.SoTietDaXep = Int32.Parse(totalLesson);
                            tkb.Thu = day;
                            tkb.TietBD = Int32.Parse(startLesson);
                            tkb.SoTiet = Int32.Parse(lessonNumber);
                            tkb.TietHoc = lessonTime;
                            tkb.MaCBGD = lecturerId;
                            tkb.TenCBGD = fullName;
                            tkb.SiSoTKB = Int32.Parse(studentNumber);
                            tkb.Trong = Int32.Parse(freeSlot);
                            tkb.TinhTrangLHP = state;
                            tkb.TuanHoc2 = learnWeek;
                            tkb.ThuS = Int32.Parse(day2);
                            tkb.TietS = Int32.Parse(startLesson2);
                            tkb.SoSVDK = Int32.Parse(studentRegisteredNumber);
                            tkb.TuanBD = Int32.Parse(startWeek);
                            tkb.TuanKT = Int32.Parse(endWeek);
                            tkb.GhiChu1 = note1;
                            tkb.GhiChu2 = note2;

                            var tkGv = model.TaiKhoan.FirstOrDefault(f => f.Ma.ToLower().Equals(lecturerId.ToLower()));
                            if (tkGv != null)
                                tkb.ID_TaiKhoan = tkGv.ID;

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
            var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();

            return PartialView("_Export", tkb);
        }

        public string ValidateColumns(DataTable dt)
        {
            // Declare the valid column names
            string[] validColumns = {
                "MaGocLHP", "Mã MH", "Mã LHP", "Tên HP", "Số TC", "Loại HP", "Mã Lớp", "TSMH",
                "Số Tiết Đã xếp", "PH", "Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "PH_X", "Sức Chứa", "SiSoTKB", "Trống", "Tình Trạng LHP", "TuanHoc2", "ThuS",
                "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Mã Ngành", "Tên Ngành", "Ghi Chú 1", "Ghi chú 2"
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