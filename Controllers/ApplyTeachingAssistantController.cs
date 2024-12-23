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
    public class ApplyTeachingAssistantController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Mở trang đăng ký trợ giảng (TA) cho người dùng có vai trò TA.
        /// Hàm này trả về trang View để ứng viên có thể đăng ký làm trợ giảng.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "Apply" để hiển thị giao diện đăng ký trợ giảng.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult Apply() //Mở trang đăng ký TA role TA
        {
            return View("Apply");
        }

        /// <summary>
        /// Lọc thời khóa biểu dựa trên các tiêu chí và trạng thái đăng ký trợ giảng.
        /// Hàm này sẽ trả về danh sách các lớp học phần phù hợp với các thông tin như học kỳ, ngành, môn học, giảng viên, và trạng thái đăng ký.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với danh sách các lớp học phần đã lọc theo các tiêu chí và trạng thái đăng ký.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost] //Filer LHP, GV, Trạng thái trang đăng ký TA
        public ActionResult FilterChildApply(int hocky, int nganh, string mon, string gv, string trangthai) //Lọc thời khóa biểu để đky TA
        {
            var lstMon = mon.Split('#').ToList();
            var lstGv = gv.Split('#').ToList();

            if (trangthai.Equals("tatca"))
            {
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else if (trangthai.Equals("dadangky"))
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() > 0).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
            else
            {
                int idTk = Int32.Parse(Session["user-id"].ToString());
                var lstTkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0
                && w.UngTuyenTroGiang.Where(wt => wt.ID_TaiKhoan == idTk).Count() < 1).ToList();
                var tkb = lstTkb.Where(w => lstMon.Contains(w.MaMH) && lstGv.Contains(w.MaCBGD)).ToList();
                return PartialView("_FilterChildApply", tkb);
            }
        }

        /// <summary>
        /// Lọc thời khóa biểu theo học kỳ và ngành học để đăng ký trợ giảng.
        /// Hàm này sẽ trả về danh sách các lớp học phần trong học kỳ và ngành học được chỉ định, 
        /// đồng thời chỉ lấy các lớp học phần đã có đề xuất trợ giảng với trạng thái là true.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với danh sách các lớp học phần đã lọc theo học kỳ và ngành học.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult FilterParentApply(int hocky, int nganh)  //Lọc thời khóa biểu theo học kỳ để đăng ký TA
        {
            var tkb = model.LopHocPhan.Where(w => w.ID_HocKy == hocky && w.ID_Nganh == nganh && w.DeXuatTroGiang.Where(we => we.TrangThai == true).Count() > 0).ToList();
            return PartialView("_FilterParentApply", tkb);
        }

        /// <summary>
        /// Tải dữ liệu thời khóa biểu (tkb) để đăng ký trợ giảng (TA).
        /// Hàm này trả về một `PartialView` chứa giao diện để đăng ký trợ giảng, bao gồm các lớp học phần có thể đăng ký.
        /// </summary>
        /// <returns>
        /// Trả về một `PartialView` với tên "_Apply", chứa giao diện và dữ liệu thời khóa biểu để người dùng có thể đăng ký trợ giảng.
        /// </returns>
        [Authorize, SVandTARole]
        public ActionResult LoadContentApply() //Load dữ liệu tkb để đăng ký TA
        {
            return PartialView("_Apply");
        }

        /// <summary>
        /// Mở hộp điền thông tin đăng ký trợ giảng cho lớp học phần được chọn.
        /// Hàm này truy vấn thông tin lớp học phần dựa trên `id` và hiển thị hộp đăng ký trợ giảng cho lớp học phần đó.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "_OpenApply", chứa thông tin lớp học phần để người dùng có thể điền thông tin đăng ký trợ giảng.
        /// Nếu không tìm thấy lớp học phần, trả về nội dung lỗi.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult OpenApply(int id) //Mở hộp điền thông đăng ký trợ giảng cho LHP được chọn
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                return View("_OpenApply", lhp);
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Mở hộp xem chi tiết công việc của lớp học phần cần đăng ký trợ giảng.
        /// Hàm này truy vấn thông tin lớp học phần dựa trên `id` và hiển thị danh sách công việc liên quan đến lớp học phần đó.
        /// </summary>
        /// <returns>
        /// Trả về một `View` với tên "_OpenTaskDetailApply", chứa danh sách công việc của lớp học phần để người dùng có thể xem.
        /// Nếu không tìm thấy lớp học phần, trả về nội dung lỗi.
        /// </returns>
        [HttpPost]
        public ActionResult OpenTaskListDetail(int id) //Mở hộp xem chi tiết công việc LHP cần đăng ký trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(id);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                return View("_OpenTaskDetailApply", lhp.CongViec.ToList());
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }

        /// <summary>
        /// Lưu thông tin ứng tuyển trợ giảng cho lớp học phần và người ứng tuyển.
        /// Hàm này sẽ xử lý thông tin người ứng tuyển, kiểm tra tính hợp lệ, lưu trữ thông tin ứng tuyển mới hoặc cập nhật thông tin ứng tuyển đã có.
        /// Nếu có tài liệu minh chứng (hình ảnh), hệ thống sẽ lưu trữ và liên kết với thông tin ứng tuyển.
        /// Sau khi hoàn thành, một thông báo sẽ được gửi đến hệ thống.
        /// </summary>
        /// <returns>
        /// Trả về một chuỗi thông báo "SUCCESS" nếu ứng tuyển mới được lưu thành công hoặc "SUCCESS2" nếu thông tin ứng tuyển đã được cập nhật.
        /// Nếu có lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult SubmitApply(int idFORM, int idLHP, int idTK, string dienthoai, DateTime ngaysinh,
            string gioitinh, decimal tbctl, decimal drl, decimal dtk, List<HttpPostedFileBase> hamc) //Lưu thông tin ứng tuyển trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var lhp = model.LopHocPhan.Find(idLHP);
                if (lhp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");

                var tk = model.TaiKhoan.Find(idTK);
                var hp = model.LopHocPhan.Find(idLHP);
                var form = model.FormDangKyTroGiang.Find(idFORM);

                if (form == null)
                    return Content("Chi tiết lỗi: " + "Chưa mở đăng ký cho học kỳ này.");
                if (hp == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.");
                if (tk == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy dữ liệu người ứng tuyển, vui lòng đăng nhập lại.");

                if (string.IsNullOrEmpty(tk.GioiTinh) || tk.NgaySinh == null || string.IsNullOrEmpty(tk.SDT))
                {
                    tk.SDT = dienthoai;
                    tk.NgaySinh = ngaysinh;
                    tk.GioiTinh = gioitinh;

                    model.Entry(tk).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();
                }
                //Chưa ứng tuyển lưu mới
                if (lhp.UngTuyenTroGiang.FirstOrDefault(f => f.ID_TaiKhoan == idTK) == null)
                {
                    var ut = new UngTuyenTroGiang();
                    ut.ID_FormDangKyTroGiang = idFORM;
                    ut.ID_LopHocPhan = idLHP;
                    ut.ID_TaiKhoan = idTK;
                    ut.MSSV = tk.Ma;
                    ut.Email = tk.Email;
                    ut.HoTen = tk.HoTen;
                    ut.SoDienThoai = dienthoai;
                    ut.NgaySinh = ngaysinh;
                    ut.GioiTinh = gioitinh;
                    ut.DiemRL = drl;
                    ut.DiemTBTL = tbctl;
                    ut.DiemTKMH = dtk;

                    string path = "";
                    string pathDirectory = "";
                    string strListImages = "";
                    int i = 0;
                    if (hamc != null)
                    {
                        foreach (var item in hamc)
                        {
                            if (item != null)
                            {
                                if (item.ContentLength > 0)
                                {
                                    i++;
                                    string fileName = item.FileName;
                                    int indexTypeFile = fileName.LastIndexOf(".");
                                    string fileType = fileName.Substring(indexTypeFile, item.FileName.Length - indexTypeFile);

                                    pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP));
                                    if (!Directory.Exists(pathDirectory))
                                    {
                                        Directory.CreateDirectory(pathDirectory);
                                    }
                                    path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP), tk.Ma + "-MC" + i + fileType);
                                    item.SaveAs(path);
                                    strListImages += "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i + fileType + "#";
                                }
                            }
                        }
                    }

                    ut.HinhAnhMinhChung = strListImages.Substring(0, strListImages.Length - 1); ;
                    ut.TrangThai = false;

                    model.UngTuyenTroGiang.Add(ut);
                    model.SaveChanges();

                    var thongbao = new ThongBao()
                    {
                        TieuDe = "Ứng tuyển trợ giảng.",
                        NoiDung = tk.HoTen + " - " + tk.Ma + " đã ứng tuyển vào Lớp " + lhp.MaLHP + ".",
                        ThoiGian = DateTime.Now,
                        DaDoc = false,
                        ForRole = "3",
                    };
                    model.ThongBao.Add(thongbao);
                    model.SaveChanges();

                    model = new CongTacTroGiangKhoaCNTTEntities();
                    return Content("SUCCESS");
                }
                else //Đã ứng tuyển - cập nhật lại tt ut
                {
                    var ut = lhp.UngTuyenTroGiang.FirstOrDefault(f => f.ID_TaiKhoan == idTK);
                    ut.MSSV = tk.Ma;
                    ut.Email = tk.Email;
                    ut.HoTen = tk.HoTen;
                    ut.SoDienThoai = dienthoai;
                    ut.NgaySinh = ngaysinh;
                    ut.GioiTinh = gioitinh;
                    ut.DiemRL = drl;
                    ut.DiemTBTL = tbctl;
                    ut.DiemTKMH = dtk;

                    string path = "";
                    string pathDirectory = "";
                    string strListImages = "";
                    string lstAnhCu = ut.HinhAnhMinhChung;

                    int i = 0;
                    if (hamc != null)
                    {
                        foreach (var item in hamc)
                        {
                            if (item != null)
                            {
                                if (item.ContentLength > 0)
                                {
                                    string fileName = item.FileName;
                                    int indexTypeFile = fileName.LastIndexOf(".");
                                    string fileType = fileName.Substring(indexTypeFile, item.FileName.Length - indexTypeFile);

                                    i++;
                                    if (!string.IsNullOrEmpty(lstAnhCu))
                                    {
                                        for (int j = 0; j < lstAnhCu.Split('#').Count(); j++)
                                        {
                                            var check = false;
                                            foreach (var items in lstAnhCu.Split('#').ToList())
                                            {
                                                var url = "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i;
                                                var nUrl = items.Substring(0, url.Length);

                                                if (url.Equals(nUrl))
                                                {
                                                    i++;
                                                    check = true;
                                                    break;
                                                }
                                            }
                                            if (check == false)
                                                break;
                                        }
                                    }
                                    pathDirectory = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP));
                                    if (!Directory.Exists(pathDirectory))
                                    {
                                        Directory.CreateDirectory(pathDirectory);
                                    }
                                    path = Path.Combine(Server.MapPath("~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP), tk.Ma + "-MC" + i + fileType);
                                    item.SaveAs(path);
                                    strListImages += "~/Content/HinhAnhMinhChung/HK" + form.HocKy.TenHocKy + "/LHP" + hp.MaLHP + "/" + tk.Ma + "-MC" + i + fileType + "#";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(strListImages))
                        ut.HinhAnhMinhChung = (string.IsNullOrEmpty(lstAnhCu) ? "" : lstAnhCu + "#") + strListImages.Substring(0, strListImages.Length - 1);

                    model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
                    model.SaveChanges();

                    model = new CongTacTroGiangKhoaCNTTEntities();
                    return Content("SUCCESS2");
                }
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
        /// <summary>
        /// Hủy bỏ ứng tuyển trợ giảng cho lớp học phần và tài khoản người ứng tuyển.
        /// Hàm này tìm kiếm thông tin ứng tuyển của người dùng dựa trên ID form đăng ký, ID tài khoản, và ID lớp học phần.
        /// Nếu thông tin ứng tuyển tồn tại, hệ thống sẽ xóa thông tin ứng tuyển và các đánh giá phỏng vấn liên quan.
        /// Sau khi xóa thành công, trả về thông báo "SUCCESS".
        /// </summary>
        /// <returns>
        /// Trả về "SUCCESS" nếu quá trình hủy ứng tuyển thành công. Nếu gặp lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult CancelApply(int idFORM, int idTK, int idLHP) //Mở hộp xem chi tiết công việc LHP cần đăng ký trợ giảng
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var ut = model.UngTuyenTroGiang.FirstOrDefault(f => f.ID_FormDangKyTroGiang == idFORM && f.ID_TaiKhoan == idTK && f.ID_LopHocPhan == idLHP);
                if (ut == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy thông tin ứng tuyển trợ giảng.");

                model.DanhGiaPhongVan.RemoveRange(ut.DanhGiaPhongVan);
                model.UngTuyenTroGiang.Remove(ut);
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
        /// Xóa hình ảnh minh chứng trong thông tin ứng tuyển trợ giảng.
        /// Hàm này tìm kiếm thông tin ứng tuyển của người dùng dựa trên ID form đăng ký, ID tài khoản, và ID lớp học phần.
        /// Sau đó, nếu tìm thấy hình ảnh trong danh sách, hệ thống sẽ xóa hình ảnh đó khỏi thư mục lưu trữ và cập nhật lại thông tin trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>
        /// Trả về "SUCCESS" nếu quá trình xóa hình ảnh thành công. Nếu gặp lỗi, trả về thông báo lỗi chi tiết.
        /// </returns>
        [Authorize, SVandTARole]
        [HttpPost]
        public ActionResult DeleteImageApply(int idFORM, int idTK, int idLHP, string url) //Xóa hình ảnh minh chứng apply hiện có
        {
            try
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var ut = model.UngTuyenTroGiang.FirstOrDefault(f => f.ID_FormDangKyTroGiang == idFORM && f.ID_TaiKhoan == idTK && f.ID_LopHocPhan == idLHP);
                if (ut == null)
                    return Content("Chi tiết lỗi: " + "Không tìm thấy thông tin ứng tuyển trợ giảng.");

                if (System.IO.File.Exists(Path.Combine(Server.MapPath(url))))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath(url)));
                }

                var lstImgCu = ut.HinhAnhMinhChung.Split('#').ToList();
                var lstUrlDelete = url.Split('#').ToList();

                string sortImg = "";
                if (lstImgCu.Count > 0)
                {
                    if (lstUrlDelete.Count > 0)
                        foreach (var imgDelete in lstUrlDelete)
                            lstImgCu.Remove(imgDelete);

                    foreach (var f in lstImgCu)
                        sortImg += f.ToString() + "#";

                    if (sortImg.Length > 0)
                        sortImg = sortImg.Substring(0, sortImg.Length - 1);
                }
                ut.HinhAnhMinhChung = sortImg;

                model.Entry(ut).State = System.Data.Entity.EntityState.Modified;
                model.SaveChanges();
                model = new CongTacTroGiangKhoaCNTTEntities();

                return Content("SUCCESS");
            }
            catch (Exception Ex)
            {
                return Content("Chi tiết lỗi: " + Ex.Message);
            }
        }
    }
}