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
using QuanLyCongTacTroGiangKhoaCNTT.Models;

namespace QuanLyCongTacTroGiangKhoaCNTT.Middlewall
{
    public class TAandGVRole : ActionFilterAttribute
    {
        CongTacTroGiangKhoaCNTTEntities model = new CongTacTroGiangKhoaCNTTEntities();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }
            else
            {
                model = new CongTacTroGiangKhoaCNTTEntities();
                var users = model.TaiKhoan.FirstOrDefault(f => f.Email.ToLower().Equals(filterContext.HttpContext.User.Identity.Name.ToLower()));
                if (users != null)
                {
                    int roleId = Int32.Parse(users.AspNetUsers.AspNetRoles.First().ID);

                    filterContext.HttpContext.Session["TaiKhoan"] = users;
                    filterContext.HttpContext.Session["user-id"] = users.ID;
                    filterContext.HttpContext.Session["user-email"] = users.Email;
                    filterContext.HttpContext.Session["user-name"] = users.HoTen;
                    filterContext.HttpContext.Session["user-role-name"] = users.AspNetUsers.AspNetRoles.First().Name;
                    filterContext.HttpContext.Session["user-role-id"] = roleId;


                    if (users.ID_Nganh == null)
                    {
                        filterContext.HttpContext.Session["user-update-info"] = false;
                        filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_Layout.cshtml";

                        filterContext.Result = new RedirectResult("~/Dashboard/Index");
                    }
                    else
                    {
                        filterContext.HttpContext.Session["user-update-info"] = true;

                        if (roleId == 1)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_StudentLayout.cshtml";
                        else if (roleId == 2)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TeacherLayout.cshtml";
                        else if (roleId == 3 || roleId == 5)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_DepartmentLayout.cshtml";
                        else if (roleId == 4)
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_TALayout.cshtml";
                        else
                            filterContext.HttpContext.Session["layout"] = "~/Views/Shared/_Layout.cshtml";

                        if (roleId != 2 && roleId != 4)
                            filterContext.Result = new RedirectResult("~/Dashboard/Index");
                    }
                }

                return;
            }
        }
    }
}