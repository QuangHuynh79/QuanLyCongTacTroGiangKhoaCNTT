using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ClassSectionTest
    {
        ClassSectionController classSectionController = new ClassSectionController();

        [TestMethod]
        public void TestRegister()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31194@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            classSectionController.ControllerContext = mockControllerContext.Object;

            ViewResult result = classSectionController.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestOpenSuggest()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31194@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            classSectionController.ControllerContext = mockControllerContext.Object;

            ContentResult result = classSectionController.OpenSuggest(0) as ContentResult;

            Assert.AreEqual("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.", result.Content);
        }

        [TestMethod]
        public void TestOpenTaskList()
        {
            var mockControllerContext = new Mock<ControllerContext>();

            var mockSession = new Mock<HttpSessionStateBase>();

            GenericPrincipal principal = new GenericPrincipal(new
            GenericIdentity("quang.197pm31194@vanlanguni.vn"), null); Thread.CurrentPrincipal = principal;

            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            classSectionController.ControllerContext = mockControllerContext.Object;

            ContentResult result = classSectionController.OpenTaskList(0) as ContentResult;

            Assert.AreEqual("Chi tiết lỗi: Lớp học phần đã bị xóa hoặc không tồn tại trên hệ thống.", result.Content);
        }
    }
}
