﻿using System;
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

namespace QuanLyCongTacTroGiangKhoaCNTT.Middlewall
{
    public class Loginverification : ActionFilterAttribute
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult { Data = "SystemLoginAgain", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                {
                    var role = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(filterContext.HttpContext.User.Identity.Name.ToLower()));
                    if (role != null)
                    {
                        filterContext.HttpContext.Session["user-id"] = role.ID;
                        filterContext.HttpContext.Session["user-email"] = role.Email;
                        filterContext.HttpContext.Session["user-name"] = role.HoTen;
                        filterContext.HttpContext.Session["user-role-name"] = role.Quyen.name;
                        filterContext.HttpContext.Session["user-role-id"] = role.ID_Quyen;

                        if (role.ID_Quyen == 2)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_StudentLayout.cshtml";
                        else if (role.ID_Quyen == 3)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TeacherLayout.cshtml";
                        else if (role.ID_Quyen == 4)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_DepartmentLayout.cshtml";
                        else if (role.ID_Quyen == 5)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TALayout.cshtml";
                        else if (role.ID_Quyen == 6)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_OfficeOfTrainingLayout.cshtml";
                        else
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_Layout.cshtml";
                    }

                    filterContext.Result = new RedirectResult("~/Dashboard/Index");
                    return;
                }
            }
        }
    }
}