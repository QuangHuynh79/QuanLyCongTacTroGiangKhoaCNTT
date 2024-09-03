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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                            var lstTkb = new List<ThoiKhoaBieu>();
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

                                var tkb = new ThoiKhoaBieu();
                                tkb.ID_HocKy = hocky;
                                tkb.ID_Nganh = nganh;
                                tkb.MaGocLHP = originalId;
                                tkb.MaMH = subjectId;
                                tkb.MaLHP = classSectionid;
                                tkb.TenHP = name;
                                tkb.SoTC = credits;
                                tkb.LoaiHP = type;
                                tkb.MaLop = malop;
                                tkb.TSMH = minimumStudent;
                                tkb.SoTietDaXep = totalLesson;
                                tkb.PH = room2;
                                tkb.Thu = day;
                                tkb.TietBD = startLesson;
                                tkb.SoTiet = lessonNumber;
                                tkb.TietHoc = lessonTime;
                                tkb.Phong = roomId;
                                tkb.MaCBGD = lecturerId;
                                tkb.TenCBGD = fullName;
                                tkb.PH_X = roomType;
                                tkb.SucChua = capacity;
                                tkb.SiSoTKB = studentNumber;
                                tkb.Trong = freeSlot;
                                tkb.TinhTrangLHP = state;
                                tkb.TuanHoc2 = learnWeek;
                                tkb.ThuS = day2;
                                tkb.TietS = startLesson2;
                                tkb.SoSVDK = studentRegisteredNumber;
                                tkb.TuanBD = startWeek;
                                tkb.TuanKT = endWeek;
                                tkb.MaNganh = nganhdb.MaNganh;
                                tkb.TenNganh = nganhdb.TenNganh;
                                tkb.GhiChu1 = note1;
                                tkb.GhiChu2 = note2;

                                lstTkb.Add(tkb);

                            }
                            model.ThoiKhoaBieu.AddRange(lstTkb);
                            model.SaveChanges();
                        }
                        else //cập nhật (addnew)
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

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim();
                            }

                            string isValid = ValidateColumns(dt);
                            if (isValid != null)
                                return Content("Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                            var rowcount = tkb.Count();
                            if (rowcount < dt.Rows.Count)
                            {
                                List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();
                                var lstTkb = new List<ThoiKhoaBieu>();
                                for (int i = 0; i < rowcount; i++)
                                {
                                    DataRow data = dt.Rows[i];
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

                                    if (!tkb[i].MaGocLHP.Equals(originalId.ToString().ToLower().Trim()))
                                        tkb[i].MaGocLHP = originalId.ToString().Trim();
                                    if (!tkb[i].MaMH.Equals(subjectId.ToString().ToLower().Trim()))
                                        tkb[i].MaMH = subjectId.ToString().Trim();
                                    if (!tkb[i].MaLHP.Equals(classSectionid.ToString().ToLower().Trim()))
                                        tkb[i].MaLHP = classSectionid.ToString().Trim();
                                    if (!tkb[i].TenHP.ToLower().Equals(name.ToString().ToLower().Trim()))
                                        tkb[i].TenHP = name.ToString().Trim();
                                    if (!tkb[i].SoTC.ToLower().Equals(credits.ToString().ToLower().Trim()))
                                        tkb[i].SoTC = credits.ToString().Trim();
                                    if (!tkb[i].LoaiHP.ToLower().Equals(type.ToString().ToLower().Trim()))
                                        tkb[i].LoaiHP = type.ToString().Trim();
                                    if (!tkb[i].MaLop.ToLower().Equals(malop.ToLower().Trim()))
                                        tkb[i].MaLop = malop;
                                    if (!tkb[i].TSMH.ToLower().Equals(minimumStudent.ToString().ToLower().Trim()))
                                        tkb[i].TSMH = minimumStudent.ToString().Trim();
                                    if (!tkb[i].SoTietDaXep.ToLower().Equals(totalLesson.ToString().ToLower().Trim()))
                                        tkb[i].SoTietDaXep = totalLesson.ToString().Trim();
                                    if (!tkb[i].PH.ToLower().Equals(room2.ToString().ToLower().Trim()))
                                        tkb[i].PH = room2.ToString().Trim();
                                    if (!tkb[i].Thu.ToLower().Equals(day.ToString().ToLower().Trim()))
                                        tkb[i].Thu = day.ToString().Trim();
                                    if (!tkb[i].TietBD.ToLower().Equals(startLesson.ToString().ToLower().Trim()))
                                        tkb[i].TietBD = startLesson.ToString().Trim();
                                    if (!tkb[i].SoTiet.ToLower().Equals(lessonNumber.ToString().ToLower().Trim()))
                                        tkb[i].SoTiet = lessonNumber.ToString().Trim();
                                    if (!tkb[i].TietHoc.ToLower().Equals(lessonTime.ToString().ToLower().Trim()))
                                        tkb[i].TietHoc = lessonTime.ToString().Trim();
                                    if (!tkb[i].Phong.ToLower().Equals(roomId.ToString().ToLower().Trim()))
                                        tkb[i].Phong = roomId.ToString().Trim();
                                    if (!tkb[i].MaCBGD.ToLower().Equals(lecturerId.ToString().ToLower().Trim()))
                                        tkb[i].MaCBGD = lecturerId.ToString().Trim();
                                    if (!tkb[i].TenCBGD.ToLower().Equals(fullName.ToString().ToLower().Trim()))
                                        tkb[i].TenCBGD = fullName.ToString().Trim();
                                    if (!tkb[i].PH_X.ToLower().Equals(roomType.ToString().ToLower().Trim()))
                                        tkb[i].PH_X = roomType.ToString().Trim();
                                    if (!tkb[i].SucChua.ToLower().Equals(capacity.ToString().ToLower().Trim()))
                                        tkb[i].SucChua = capacity.ToString().Trim();
                                    if (!tkb[i].SiSoTKB.ToLower().Equals(studentNumber.ToString().ToLower().Trim()))
                                        tkb[i].SiSoTKB = studentNumber.ToString().Trim();
                                    if (!tkb[i].Trong.ToLower().Equals(freeSlot.ToString().ToLower().Trim()))
                                        tkb[i].Trong = freeSlot.ToString().Trim();
                                    if (!tkb[i].TinhTrangLHP.ToLower().Equals(state.ToString().ToLower().Trim()))
                                        tkb[i].TinhTrangLHP = state.ToString().Trim();
                                    if (!tkb[i].TuanHoc2.ToLower().Equals(learnWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanHoc2 = learnWeek.ToString().Trim();
                                    if (!tkb[i].ThuS.ToLower().Equals(day2.ToString().ToLower().Trim()))
                                        tkb[i].ThuS = day2.ToString().Trim();
                                    if (!tkb[i].TietS.ToLower().Equals(startLesson2.ToString().ToLower().Trim()))
                                        tkb[i].TietS = startLesson2.ToString().Trim();
                                    if (!tkb[i].SoSVDK.ToLower().Equals(studentRegisteredNumber.ToString().ToLower().Trim()))
                                        tkb[i].SoSVDK = studentRegisteredNumber.ToString().Trim();
                                    if (!tkb[i].TuanBD.ToLower().Equals(startWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanBD = startWeek.ToString().Trim();
                                    if (!tkb[i].TuanKT.ToLower().Equals(endWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanKT = endWeek.ToString().Trim();
                                    if (!tkb[i].GhiChu1.ToLower().Equals(note1.ToString().ToLower().Trim()))
                                        tkb[i].GhiChu1 = note1.ToString().Trim();
                                    if (!tkb[i].GhiChu2.ToLower().Equals(note2.ToString().ToLower().Trim()))
                                        tkb[i].GhiChu2 = note2.ToString().Trim();

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

                                    if (!tkb[i].MaGocLHP.Equals(originalId.ToString().ToLower().Trim()))
                                        tkb[i].MaGocLHP = originalId.ToString().Trim();
                                    if (!tkb[i].MaMH.Equals(subjectId.ToString().ToLower().Trim()))
                                        tkb[i].MaMH = subjectId.ToString().Trim();
                                    if (!tkb[i].MaLHP.Equals(classSectionid.ToString().ToLower().Trim()))
                                        tkb[i].MaLHP = classSectionid.ToString().Trim();
                                    if (!tkb[i].TenHP.ToLower().Equals(name.ToString().ToLower().Trim()))
                                        tkb[i].TenHP = name.ToString().Trim();
                                    if (!tkb[i].SoTC.ToLower().Equals(credits.ToString().ToLower().Trim()))
                                        tkb[i].SoTC = credits.ToString().Trim();
                                    if (!tkb[i].LoaiHP.ToLower().Equals(type.ToString().ToLower().Trim()))
                                        tkb[i].LoaiHP = type.ToString().Trim();
                                    if (!tkb[i].MaLop.ToLower().Equals(malop.ToLower().Trim()))
                                        tkb[i].MaLop = malop;
                                    if (!tkb[i].TSMH.ToLower().Equals(minimumStudent.ToString().ToLower().Trim()))
                                        tkb[i].TSMH = minimumStudent.ToString().Trim();
                                    if (!tkb[i].SoTietDaXep.ToLower().Equals(totalLesson.ToString().ToLower().Trim()))
                                        tkb[i].SoTietDaXep = totalLesson.ToString().Trim();
                                    if (!tkb[i].PH.ToLower().Equals(room2.ToString().ToLower().Trim()))
                                        tkb[i].PH = room2.ToString().Trim();
                                    if (!tkb[i].Thu.ToLower().Equals(day.ToString().ToLower().Trim()))
                                        tkb[i].Thu = day.ToString().Trim();
                                    if (!tkb[i].TietBD.ToLower().Equals(startLesson.ToString().ToLower().Trim()))
                                        tkb[i].TietBD = startLesson.ToString().Trim();
                                    if (!tkb[i].SoTiet.ToLower().Equals(lessonNumber.ToString().ToLower().Trim()))
                                        tkb[i].SoTiet = lessonNumber.ToString().Trim();
                                    if (!tkb[i].TietHoc.ToLower().Equals(lessonTime.ToString().ToLower().Trim()))
                                        tkb[i].TietHoc = lessonTime.ToString().Trim();
                                    if (!tkb[i].Phong.ToLower().Equals(roomId.ToString().ToLower().Trim()))
                                        tkb[i].Phong = roomId.ToString().Trim();
                                    if (!tkb[i].MaCBGD.ToLower().Equals(lecturerId.ToString().ToLower().Trim()))
                                        tkb[i].MaCBGD = lecturerId.ToString().Trim();
                                    if (!tkb[i].TenCBGD.ToLower().Equals(fullName.ToString().ToLower().Trim()))
                                        tkb[i].TenCBGD = fullName.ToString().Trim();
                                    if (!tkb[i].PH_X.ToLower().Equals(roomType.ToString().ToLower().Trim()))
                                        tkb[i].PH_X = roomType.ToString().Trim();
                                    if (!tkb[i].SucChua.ToLower().Equals(capacity.ToString().ToLower().Trim()))
                                        tkb[i].SucChua = capacity.ToString().Trim();
                                    if (!tkb[i].SiSoTKB.ToLower().Equals(studentNumber.ToString().ToLower().Trim()))
                                        tkb[i].SiSoTKB = studentNumber.ToString().Trim();
                                    if (!tkb[i].Trong.ToLower().Equals(freeSlot.ToString().ToLower().Trim()))
                                        tkb[i].Trong = freeSlot.ToString().Trim();
                                    if (!tkb[i].TinhTrangLHP.ToLower().Equals(state.ToString().ToLower().Trim()))
                                        tkb[i].TinhTrangLHP = state.ToString().Trim();
                                    if (!tkb[i].TuanHoc2.ToLower().Equals(learnWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanHoc2 = learnWeek.ToString().Trim();
                                    if (!tkb[i].ThuS.ToLower().Equals(day2.ToString().ToLower().Trim()))
                                        tkb[i].ThuS = day2.ToString().Trim();
                                    if (!tkb[i].TietS.ToLower().Equals(startLesson2.ToString().ToLower().Trim()))
                                        tkb[i].TietS = startLesson2.ToString().Trim();
                                    if (!tkb[i].SoSVDK.ToLower().Equals(studentRegisteredNumber.ToString().ToLower().Trim()))
                                        tkb[i].SoSVDK = studentRegisteredNumber.ToString().Trim();
                                    if (!tkb[i].TuanBD.ToLower().Equals(startWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanBD = startWeek.ToString().Trim();
                                    if (!tkb[i].TuanKT.ToLower().Equals(endWeek.ToString().ToLower().Trim()))
                                        tkb[i].TuanKT = endWeek.ToString().Trim();
                                    if (!tkb[i].GhiChu1.ToLower().Equals(note1.ToString().ToLower().Trim()))
                                        tkb[i].GhiChu1 = note1.ToString().Trim();
                                    if (!tkb[i].GhiChu2.ToLower().Equals(note2.ToString().ToLower().Trim()))
                                        tkb[i].GhiChu2 = note2.ToString().Trim();

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

                        foreach (DataColumn col in dt.Columns)
                        {
                            col.ColumnName = col.ColumnName.Trim();
                        }

                        string isValid = ValidateColumns(dt);
                        if (isValid != null)
                            return Content($"Có vẻ như bạn đã sai hoặc thiếu tên cột [" + isValid + "], vui lòng kiểm tra lại tệp tin!");

                        List<ThoiKhoaBieu> lstTemp = new List<ThoiKhoaBieu>();

                        var lstTkb = new List<ThoiKhoaBieu>();
                        foreach (DataRow data in dt.Rows)
                        {
                            if (!data["Mã Ngành"].ToString().ToLower().Equals(nganhdb.MaNganh.ToLower())
                                && !data["Tên Ngành"].ToString().ToLower().Equals(nganhdb.TenNganh.ToLower()))
                                continue;

                            string originalId = data["MaGocLHP"].ToString(); //
                            string subjectId = data["Mã MH"].ToString(); //
                            string classSectionid = data["Mã LHP"].ToString(); //
                            string name = data["Tên HP"].ToString(); //
                            string credits = data["Số TC"].ToString(); //
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

                            var tkb = new ThoiKhoaBieu();
                            tkb.ID_HocKy = hocky;
                            tkb.ID_Nganh = nganh;
                            tkb.MaGocLHP = originalId;
                            tkb.MaMH = subjectId;
                            tkb.MaLHP = classSectionid;
                            tkb.TenHP = name;
                            tkb.SoTC = credits;
                            tkb.LoaiHP = type;
                            tkb.MaLop = malop;
                            tkb.TSMH = minimumStudent;
                            tkb.SoTietDaXep = totalLesson;
                            tkb.PH = room2;
                            tkb.Thu = day;
                            tkb.TietBD = startLesson;
                            tkb.SoTiet = lessonNumber;
                            tkb.TietHoc = lessonTime;
                            tkb.Phong = roomId;
                            tkb.MaCBGD = lecturerId;
                            tkb.TenCBGD = fullName;
                            tkb.PH_X = roomType;
                            tkb.SucChua = capacity;
                            tkb.SiSoTKB = studentNumber;
                            tkb.Trong = freeSlot;
                            tkb.TinhTrangLHP = state;
                            tkb.TuanHoc2 = learnWeek;
                            tkb.ThuS = day2;
                            tkb.TietS = startLesson2;
                            tkb.SoSVDK = studentRegisteredNumber;
                            tkb.TuanBD = startWeek;
                            tkb.TuanKT = endWeek;
                            tkb.MaNganh = nganhdb.MaNganh;
                            tkb.TenNganh = nganhdb.TenNganh;
                            tkb.GhiChu1 = note1;
                            tkb.GhiChu2 = note2;

                            lstTkb.Add(tkb);
                            //model.ThoiKhoaBieu.Add(tkb);
                            //model.SaveChanges();

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