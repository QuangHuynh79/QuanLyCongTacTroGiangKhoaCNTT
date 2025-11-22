using DocumentFormat.OpenXml.EMMA;
using QuanLyCongTacTroGiangKhoaCNTT.Middlewall;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class TaskManagementController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Hàm này mở trang quản lý công việc.
        /// </summary>
        /// <returns>
        /// Trả về view "TaskList" để hiển thị giao diện quản lý công việc.
        /// </returns>
        [Authorize, TAandGVRole]
        public ActionResult TaskList() //Quản lý công việc
        {
            return View("TaskList");
        }
        /// <summary>
        /// Hàm này tải dữ liệu cho trang quản lý công việc tùy theo vai trò người dùng.
        /// Nếu người dùng có vai trò là TA, trả về partial view "_TaskListTA", nếu là GV, trả về partial view "_TaskListGV".
        /// </summary>
        /// <returns>
        /// Trả về partial view tương ứng với vai trò của người dùng. 
        /// Nếu người dùng có vai trò TA, trả về "_TaskListTA", nếu người dùng có vai trò GV, trả về "_TaskListGV".
        /// </returns>
        [Authorize, TAandGVRole]
        public ActionResult LoadContentTaskList() //Load dữ liệu quản lý công việc
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            if (role == 4)
                return PartialView("_TaskListTA"); //Role TA
            else
                return PartialView("_TaskListGV"); //Role GV
        }
        /// <summary>
        /// Hàm này lọc thông tin lớp học phần và trả về thông tin của lớp học phần được chọn.
        /// Dựa trên ID lớp học phần được truyền vào, hàm tìm kiếm thông tin và trả về partial view với dữ liệu lớp học phần.
        /// </summary>
        /// <returns>
        /// Trả về partial view "_FilterTaskList" với thông tin lớp học phần tương ứng.
        /// </returns>
        [Authorize, TAandGVRole]
        [HttpPost] //Lọc danh sách công việc
        public ActionResult FilterTaskList(int lophocphan)
        {
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            var task = model.LopHocPhan.FirstOrDefault(f => f.ID == lophocphan);

            return PartialView("_FilterTaskList", task);
        }
        /// <summary>
        /// Hàm này lọc danh sách lớp học phần trong quản lý công việc theo học kỳ, phân biệt giữa vai trò trợ giảng (TA) và giảng viên (GV).
        /// - Nếu vai trò là TA, lọc lớp học phần mà TA được phân công công việc.
        /// - Nếu vai trò là GV, lọc lớp học phần mà GV giảng dạy và có công việc.
        /// </summary>
        /// <returns>
        /// Trả về partial view "_FilterHocKyTaskListTA" (dành cho TA) hoặc "_FilterHocKyTaskListGV" (dành cho GV) với danh sách lớp học phần theo học kỳ.
        /// </returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult FilterHocKyTaskList(int hocky) //lọc danh sách lớp học phần trong quản lý công việc theo học kỳ
        {
            int role = Int32.Parse(Session["user-role-id"].ToString());
            var taikhoan = Session["TaiKhoan"] as TaiKhoan;
            var ma = string.IsNullOrEmpty(taikhoan.Ma) ? "" : taikhoan.Ma.ToLower();

            if (role == 4)
            {
                int idTk = taikhoan.ID;
                var tasks = model.LopHocPhan.Where(w => w.CongViec.Where(wt => wt.ID_TaiKhoan == idTk).Count() > 0 && w.ID_HocKy == hocky).ToList();
                var task = tasks.Where(w => w.DeXuatTroGiang.First().TrangThai == true).ToList();
                return PartialView("_FilterHocKyTaskListTA", task); //Role TA
            }
            else
            {
                var tasks = model.LopHocPhan.Where(w => w.MaCBGD.ToLower().Equals(ma) && w.CongViec.Count > 0 && w.ID_HocKy == hocky).ToList();
                var task = tasks.Where(w => w.DeXuatTroGiang.First().TrangThai == true).ToList();
                return PartialView("_FilterHocKyTaskListGV", task); //Role GV
            }
        }
        /// <summary>
        /// Hàm này lưu thông tin chỉnh sửa chi tiết công việc, bao gồm trạng thái công việc, ghi chú, và hình ảnh minh chứng.
        /// - Nếu người dùng là giảng viên (role = "gv"), lưu trạng thái hoàn thành hoặc chưa hoàn thành và ghi chú.
        /// - Nếu người dùng là sinh viên (role != "gv"), lưu trạng thái công việc (loại bỏ tiền tố "task" nếu có) và ghi chú.
        /// - Nếu có hình ảnh minh chứng mới, lưu hình ảnh đó vào thư mục tương ứng và xóa hình ảnh cũ nếu yêu cầu.
        /// </summary>
        /// <returns>Trả về thông báo "SUCCESS" nếu lưu thành công, hoặc thông báo lỗi nếu có lỗi xảy ra.</returns>
        [Authorize, TAandGVRole]
        [HttpPost] //Lưu cập nhật trạng thái công việc
        public ActionResult SubmitEditTaskDetail(int id, string role, string trangthai, string ghichu, HttpPostedFileBase hamc, string deleteImg)
        {
            try
            {
                var tk = Session["Taikhoan"] as TaiKhoan;
                var cv = model.CongViec.Find(id);
                if (cv == null)
                    return Content("Chi tiết lỗi: Không tìm thấy công việc tương ứng.");

                if (role.Equals("gv")) //Là giảng viên thì lưu tình trạng
                {
                    if (trangthai.Equals("hoanthanh") || trangthai.Equals("chuahoanthanh"))
                    {
                        cv.TrangThai = "hoanthanh";
                    }
                    cv.KetQuaCongViec = trangthai;
                    cv.GhiChu = ghichu;
                }
                else // Là sinh viên thì lưu trạng thái
                {
                    if (trangthai.IndexOf("task") == -1) //Dạng kéo thả task sẽ kèm chữ task phía trước để phân biệt
                        cv.GhiChu = ghichu;
                    cv.TrangThai = trangthai.Replace("task", ""); //Xóa chữ task phía trước trạng thái để lưu trạng thái

                    if (!string.IsNullOrEmpty(deleteImg)) //Check xóa img cũ k
                    {
                        bool deleteImgs = Convert.ToBoolean(deleteImg); // Convert string to bool
                        if (deleteImgs)
                        {
                            string url = cv.HinhAnhMinhChung;
                            cv.HinhAnhMinhChung = null;
                            if (System.IO.File.Exists(Path.Combine(Server.MapPath(url))))
                            {
                                System.IO.File.Delete(Path.Combine(Server.MapPath(url)));
                            }
                        }

                        //Lưu hình ảnh minh chứng path Content/HinhAnhMinhChungCongViec/LHP/MSSV-MC1
                        string path = "";
                        string pathDirectory = "";
                        int i = 0;
                        if (hamc != null)
                        {
                            if (hamc.ContentLength > 0)
                            {
                                i++;
                                string fileName = hamc.FileName;
                                int indexTypeFile = fileName.LastIndexOf(".");
                                string fileType = fileName.Substring(indexTypeFile, hamc.FileName.Length - indexTypeFile);

                                pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP));
                                if (!Directory.Exists(pathDirectory))
                                {
                                    Directory.CreateDirectory(pathDirectory);
                                }
                                path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP), tk.Ma + "-MC" + i + fileType);
                                hamc.SaveAs(path);

                                cv.HinhAnhMinhChung = "~/Content/HinhAnhMinhChungCongViec/LHP" + cv.LopHocPhan.MaLHP + "/" + tk.Ma + "-MC" + i + fileType;
                            }
                        }
                    }
                }

                model.Entry(cv).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Hàm này trả về form chi tiết công việc theo ID công việc.
        /// Tìm công việc từ cơ sở dữ liệu và trả về PartialView với thông tin chi tiết của công việc.
        /// </summary>
        /// <returns>Trả về PartialView "_TaskDetail" với thông tin chi tiết của công việc.</returns>
        [Authorize, TAandGVRole]
        [HttpPost]
        public ActionResult TaskDetail(int id) //Mở form chi tiết công việc
        {
            return PartialView("_TaskDetail", model.CongViec.Find(id));
        }
    }
}