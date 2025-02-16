﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuanLyCongTacTroGiangKhoaCNTT.Controllers;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.Security.Claims;
using System.Security.Principal;
using System.Collections.Generic;
using System.Threading;
using QuanLyCongTacTroGiangKhoaCNTT.Models;

namespace QuanLyCongTacTroGiangKhoaCNTTTest
{
    [TestClass]
    public class TeachingAssistantTest
    {
        TeachingAssistantController teachingAssistantController = new TeachingAssistantController();

        [TestMethod]
        public void TestRegister()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ViewResult result = teachingAssistantController.Register() as ViewResult;

            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void TestOpenEditRegisterNotExist()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.OpenEditRegister(0) as ContentResult;

            Assert.AreEqual("Form đăng ký không tồn tại trên hệ thống.", result.Content);
        }

        [TestMethod]
        public void TestAddRegisterDateStart()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.AddRegister(0, 0, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1)) as ContentResult;

            Assert.AreEqual("NhoHonHienTai", result.Content);
        }

        [TestMethod]
        public void TestAddRegisterDateEnd()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.AddRegister(0, 0, DateTime.Now.AddDays(1), DateTime.Now) as ContentResult;

            Assert.AreEqual("LonHonDangKy", result.Content);
        }

        [TestMethod]
        public void TestEditRegisterDateStart()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.EditRegister(0, 0, 0, DateTime.Now.AddDays(-1), DateTime.Now) as ContentResult;

            Assert.AreEqual("NhoHonHienTai", result.Content);
        }

        [TestMethod]
        public void TestEditRegisterDateEnd()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.EditRegister(0, 0, 0, DateTime.Now, DateTime.Now) as ContentResult;

            Assert.AreEqual("LonHonDangKy", result.Content);
        }

        [TestMethod]
        public void TestDeleteRegister()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.DeleteRegister(0) as ContentResult;

            Assert.AreEqual("SUCCESS", result.Content);
        }

        [TestMethod]
        public void TestAdvances()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ViewResult result = teachingAssistantController.Advances() as ViewResult;

            Assert.AreEqual("Advances", result.ViewName);
        }

        [TestMethod]
        public void TestOpenSuggest()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.OpenSuggest(0) as ContentResult;

            Assert.AreEqual("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.", result.Content);
        }

        [TestMethod]
        public void TestAcceptedAdvances()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.AcceptedAdvances(0) as ContentResult;

            Assert.AreEqual("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.", result.Content);
        }

        [TestMethod]
        public void TestRegistered()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.Registered() as ContentResult;

            Assert.AreEqual("Đã xảy ra lỗi! Vui lòng thử lại sau.", result.Content);
        }

        [TestMethod]
        public void TestOpenTaskListDetail()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31195@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            teachingAssistantController.ControllerContext = mockControllerContext.Object;

            ContentResult result = teachingAssistantController.OpenTaskListDetail(0) as ContentResult;

            Assert.AreEqual("Chi tiết lỗi: " + "Không tìm thấy lớp học phần tương ứng.", result.Content);
        }
    }
}
