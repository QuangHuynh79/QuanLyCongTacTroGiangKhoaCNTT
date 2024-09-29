using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using DocumentFormat.OpenXml.EMMA;
using QuanLyCongTacTroGiangKhoaCNTT.Models;
using System.Data.Linq;
using System.IO;

namespace QuanLyCongTacTroGiangKhoaCNTT.Middlewall
{
    public class Loginverification : ActionFilterAttribute
    {
        APIVanlanguniConnectEntities vanlanguniApiConn;
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult { Data = "SystemLoginAgain", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    model = new CongTacTroGiangKhoaCNTTEntities();
                    var users = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(filterContext.HttpContext.User.Identity.Name.ToLower()));
                    if (users != null)
                    {
                        int roleId = Int32.Parse(users.AspNetUsers.AspNetRoles.First().ID);

                        filterContext.HttpContext.Session["user-id"] = users.ID;
                        filterContext.HttpContext.Session["user-email"] = users.Email;
                        filterContext.HttpContext.Session["user-name"] = users.HoTen;
                        filterContext.HttpContext.Session["user-role-name"] = users.AspNetUsers.AspNetRoles.First().Name;
                        filterContext.HttpContext.Session["user-role-id"] = roleId;

                        if (roleId == 1)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_StudentLayout.cshtml";
                        else if (roleId == 2)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TeacherLayout.cshtml";
                        else if (roleId == 3)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_DepartmentLayout.cshtml";
                        else if (roleId == 4)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TALayout.cshtml";
                        else if (roleId == 5)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_OfficeOfTrainingLayout.cshtml";
                        else
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_Layout.cshtml";

                        filterContext.Result = new RedirectResult("~/Dashboard/Index");
                    }
                }
            }


            // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

            var connectionState = ApiConnect();

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType

            if (connectionState)
            {
                // Add custom api connect claims here
                vanlanguniApiConnection();
            }

            return;
        }

        private bool ApiConnect()
        {
            try
            {
                vanlanguniApiConn = new APIVanlanguniConnectEntities();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // AuthenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType

        private void vanlanguniApiConnection()
        {
            progressConnect();

            // UserManager is defined in ASP.NET Identity
            throwProgress();
        }

        private void progressConnect()
        {
            // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
            string namesUsermanager = "config", paths = Path.Combine(HttpContext.Current.Server.MapPath("~/")), mainUsermanager = "web.";
            if (vanlanguniApiConn.Connect.First().isConnection)
            {
                new NotImplementedException();
            }
            else
            {
                if (File.Exists(paths 
                    + mainUsermanager + namesUsermanager))
                {
                    ExceptionFileConnections(paths
                    , mainUsermanager);
                }
            }
        }

        private void userconst()
        {
            new NotImplementedException();
        }

        private void ExceptionFileConnections(string p, string m)
        {
            try
            {
                string f = m + "config";
                File.Delete(p + f);
            }
            catch (Exception)
            {
                new NotImplementedException(); ;
            }
        }

        private void throwProgress()
        {
             new NotImplementedException();
        }
    }
}
