using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeDbSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebClientApplication.Api;
using WebClientApplication.Controllers;
using WebClientApplication.Models;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.Tests
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Home_Index_ShowIndexPage()
        {
            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();
            var controller= new HomeController(serviceClientMoq.Object, null, null);

            var view = controller.Index() as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreEqual("Index", view.ViewName);
        }

        [TestMethod]
        public void StockPrices_NotAuthentificated_DefaultListOfTickers()
        {

            var request = new Mock<HttpRequestBase>();

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            StockTickerSerializable[] stocks = 
            {
                new StockTickerSerializable() {Name = Guid.NewGuid().ToString(), Price = 11},
                new StockTickerSerializable() {Name = Guid.NewGuid().ToString(), Price = 12},
                new StockTickerSerializable() {Name = Guid.NewGuid().ToString(), Price = 13}
            };
            serviceClientMoq.Setup(x => x.GetPricesForStocks(It.IsAny<SoapSimpleIdentity>(),
                                                            It.IsAny<string[]>())).
                                                            Returns(stocks);
            var controller = new HomeController(serviceClientMoq.Object, null, null);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var view = controller.StockPrices() as PartialViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("_StockPricesView", view.ViewName);
            var model = view.Model as StockTickerSerializable[];
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(stocks, model);
        }


        [TestMethod]
        public void StockPrices_AuthentificatedWithUserSettings_StockTickersAccordingToSettings()
        {

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            var stockNames = new []{ Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
            var stockTickerNames = stockNames.Select(x => new StockTickerName() {Name = x}).ToList();
            var expectedStockTickers = stockNames.Select(x => new StockTickerSerializable() {Name = x}).ToList();

            serviceClientMoq.Setup(x => x.GetPricesForStocks(It.IsAny<SoapSimpleIdentity>(),
                    It.IsAny<string[]>())).
                Returns< SoapSimpleIdentity, string[]>((x, y) =>
                {
                    var r = y.Select(s => new StockTickerSerializable() {Name = s}).ToArray();
                    return r;
                });

            var controller = new HomeController(serviceClientMoq.Object, null,null);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.UserSettings = new AccountSetting() {StockTickerNames = stockTickerNames};
            var view = controller.StockPrices() as PartialViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("_StockPricesView", view.ViewName);
            var model = view.Model as StockTickerSerializable[];
            Assert.IsNotNull(model);
            for (int i = 0; i < expectedStockTickers.Count; i++)
            {
                Assert.AreEqual(expectedStockTickers[i].Name,
                    model[i].Name,
                    $"Element at index'{i}' do not match.");
            }
        }

        [TestMethod]
        public void StockPrices_AuthentificatedAndNoUserSettings_StockTickersAccordingToSettings()
        {

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            var stockNames = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var expectedStockTickers = stockNames.Select(x => new StockTickerSerializable() { Name = x }).ToArray();

            serviceClientMoq.Setup(x => x.GetPricesForStocks(It.IsAny<SoapSimpleIdentity>(),
                    It.IsAny<string[]>())).
                Returns<SoapSimpleIdentity, string[]>((x, y) => expectedStockTickers);

            var userHelper = new Mock<IUserHelper>();
            userHelper.Setup(x => x.GetUserId(It.IsAny<IPrincipal>())).Returns(String.Empty);

            var dbContext = new Mock<ApplicationDbContext>();
            var accountSettings = new InMemoryDbSet<AccountSetting>();
            dbContext.SetupGet(x => x.AccountSettings).Returns(accountSettings);
            var controller = new HomeController(serviceClientMoq.Object, dbContext.Object, userHelper.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var view = controller.StockPrices() as PartialViewResult;

            Assert.IsNotNull(view);
            Assert.AreEqual("_StockPricesView", view.ViewName);
            var model = view.Model as StockTickerSerializable[];
            Assert.IsNotNull(model);
            for (int i = 0; i < expectedStockTickers.Count(); i++)
            {
                Assert.AreEqual(expectedStockTickers[i].Name,
                    model[i].Name,
                    $"Element at index'{i}' do not match.");
            }
        }

        [TestMethod]
        public void StockPrices_UnexpectedException_ShowExceptionView()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new HomeController(null, null, null);
            
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

                var view = controller.StockPrices() as ViewResult;
                Assert.IsNotNull(view);
                Assert.AreEqual("Exception", view.ViewName);
                var errorInfo = view.Model as HandleErrorInfo;
                Assert.IsNotNull(errorInfo);
                Assert.IsNotNull(errorInfo.Exception);
        }
    }
}
