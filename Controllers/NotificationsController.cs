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
using System.Net.Mail;

namespace QuanLyCongTacTroGiangKhoaCNTT.Controllers
{
    public class NotificationsController : Controller
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();

        /// <summary>
        /// Lấy danh sách tất cả các thông báo và hiển thị chúng trên trang chủ.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ActionResult"> chứa danh sách thông báo dưới dạng một danh sách các đối tượng ThongBao.
        /// Dữ liệu sẽ được hiển thị trên view "Index".
        /// </returns>
        public ActionResult LoadNotification() //Load thông báo
        {
            var lstNotify = model.ThongBao.ToList();
            return View("Index", lstNotify);
        }

        /// <summary>
        /// Lưu thông tin thông báo xuống db.
        /// </summary>
        /// <returns>
        /// Trả về dữ liệu thông báo được lưu xuống database
        /// </returns>
        public string SetNotification(string title, string content, string forRole, int? idTk, int sendMailType, string email, string hoten, string tenlhp, string link) //Lưu thông báo
        {
            try
            {
                var thongbao = new ThongBao()
                {
                    TieuDe = title,
                    NoiDung = content,
                    ThoiGian = DateTime.Now,
                    DaDoc = false,
                    ForRole = forRole,
                    ID_TaiKhoan = idTk
                };
                model.ThongBao.Add(thongbao);
                model.SaveChanges();

                if (sendMailType != 0)
                {
                    try
                    {
                        var mailDb = model.ThongBaoMail.Find(sendMailType);
                        if (mailDb != null)
                        {
                            string mailSend = "k.cntt-test1@vanlanguni.vn";
                            string passMailSend = "cntt@Test1";

                            string tieuDe = mailDb.TieuDe;
                            string noiDung = mailDb.NoiDung.Replace("\n", "<br><br>");

                            string mail = email;
                            string hoTen = hoten;
                            string tenLhp = tenlhp;
                            string lienKet = link;

                            tieuDe = tieuDe.Replace("<TenLopHocPhan>", tenLhp);
                            noiDung = noiDung.Replace("<HoTenSinhVien>", hoten).Replace("<TenLopHocPhan>", tenLhp).Replace("<LienKet>", "<a href=\"" + lienKet + "\">" + lienKet + "</a>");

                            if (mail.IndexOf("BCN") != -1)
                            {
                                int idNganh = Int32.Parse(mail.Split('#')[1].ToString().Trim());

                                using (MailMessage mailMessage = new MailMessage())
                                {
                                    mailMessage.From = new MailAddress(mailSend);

                                    var lstBcn = model.TaiKhoan.Where(w => w.ID_Nganh == idNganh && w.AspNetUsers.AspNetRoles.Where(ws => ws.ID.Equals("3") || ws.ID.Equals("5")).Count() > 0).ToList();
                                    foreach (var item in lstBcn)
                                        mailMessage.To.Add(item.Email);

                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Subject = tieuDe;
                                    mailMessage.Body = noiDung;

                                    using (SmtpClient smtp = new SmtpClient())
                                    {
                                        smtp.Host = "smtp-mail.outlook.com";
                                        smtp.EnableSsl = true;
                                        NetworkCredential cred = new NetworkCredential(mailSend, passMailSend);
                                        smtp.UseDefaultCredentials = true;
                                        smtp.Credentials = cred;
                                        smtp.Port = 587;

                                        smtp.Send(mailMessage);
                                    }
                                }
                            }
                            else
                            {
                                using (MailMessage mailMessage = new MailMessage())
                                {
                                    mailMessage.From = new MailAddress(mailSend);
                                    mailMessage.To.Add(mail);

                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Subject = tieuDe;
                                    mailMessage.Body = noiDung;

                                    using (SmtpClient smtp = new SmtpClient())
                                    {
                                        smtp.Host = "smtp-mail.outlook.com";
                                        smtp.EnableSsl = true;
                                        NetworkCredential cred = new NetworkCredential(mailSend, passMailSend);
                                        smtp.UseDefaultCredentials = true;
                                        smtp.Credentials = cred;
                                        smtp.Port = 587;

                                        smtp.Send(mailMessage);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                    }
                }
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                return "Chi tiết lỗi: " + Ex.Message;
            }
        }

        /// <summary>
        /// Xóa một thông báo theo ID.
        /// </summary>
        /// <returns>
        /// Trả về một đối tượng <see cref="ContentResult"> với thông báo "SUCCESS" nếu xóa thành công.
        /// Nếu không tìm thấy thông báo, trả về "SUCCESS" để báo rằng không có thông báo nào được xóa.
        /// </returns>
        public ActionResult Delete(int id) //Xóa thông báo
        {
            var tb = model.ThongBao.Find(id);
            if (tb == null)
                return Content("SUCCESS");

            model.ThongBao.Remove(tb);
            model.SaveChanges();

            return Content("SUCCESS");
        }


        /// <summary>
        /// Tìm kiếm thông báo theo nội dung.
        /// </summary>
        /// <returns>
        /// Trả về danh sách thông báo khớp với nội dung tìm kiếm theo nội dung thông báo.
        /// </returns>
        public ActionResult Search(string search) //Tìm kiếm thông báo
        {
            if (string.IsNullOrEmpty(search))
            {
                var tb = Session["list-noti-default"] as List<ThongBao>;
                return PartialView("_Search", tb);
            }
            else
            {
                var tbDefault = Session["list-noti-default"] as List<ThongBao>;
                var tb = tbDefault.Where(w => w.NoiDung.ToLower().Contains(search.ToLower()) || w.TieuDe.ToLower().Contains(search.ToLower())).ToList();
                return PartialView("_Search", tb);
            }
        }
    }
}