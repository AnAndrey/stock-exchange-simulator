using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        public void Index()
        {
            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();
            var controller= new HomeController(serviceClientMoq.Object);

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
            var controller = new HomeController(serviceClientMoq.Object);

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

            var controller = new HomeController(serviceClientMoq.Object);

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
        public void StockPrices_UnexpectedException_ShowExceptionView()
        {
            string testException = Guid.NewGuid().ToString(); 
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Throws(new Exception(testException));

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);


            var controller = new HomeController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var view =controller.StockPrices() as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreEqual("Exception", view.ViewName);
            var errorInfo = view.Model as HandleErrorInfo;
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(testException, errorInfo.Exception.Message);
        }
    }
}
