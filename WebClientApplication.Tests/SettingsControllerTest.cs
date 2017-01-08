using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
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
    public class SettingsControllerTest
    {
        [TestMethod]
        public void Settings_IndexAndNotAuthorized_RedirectToLoginPage()
        {
            var request = new Mock<HttpRequestBase>();

            var urlHelper = new Mock<UrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<string>())).Returns(String.Empty);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();
            var controller = new SettingsController(serviceClientMoq.Object, null, null, null);
            controller.Url = urlHelper.Object;
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index() as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);

        }

        [TestMethod]
        public void Settings_IndexAndAuthorized_ShowSettingsPageAndCheckedStocks()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            var controller = new SettingsController(serviceClientMoq.Object, null, null, null);

            var stockNames = new[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
            var stockTickers = stockNames.Select(x => new StockTickerSerializable() {Name = x}).ToArray();

            serviceClientMoq.Setup(x => x.GetPricesForStocks(It.IsAny<SoapSimpleIdentity>(),
                    It.IsAny<string[]>())).
                Returns(stockTickers);

            controller.UserSettings = new AccountSetting()
            {
                StockTickerNames = new[] {new StockTickerName() {Name = stockNames[0]}}
            };
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as StockTickerListModel;
            Assert.IsNotNull(model);

            for (int i = 0; i < stockNames.Count(); i++)
            {

                Assert.AreEqual(stockNames[i],
                    model.tickers[i].Name,
                    $"Element at index'{i}' do not match.");
            }
            Assert.IsTrue(model.tickers[0].IsChecked);
            Assert.IsFalse(model.tickers[1].IsChecked);
            Assert.IsFalse(model.tickers[2].IsChecked);
        }

        [TestMethod]
        public void Settings_IndexAndAuthorizedAndNoSettings_ShowSettingsPageAndAllCheckedStocks()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            var accountSettings = new InMemoryDbSet<AccountSetting>();
            var dbContext = new Mock<ApplicationDbContext>();
            dbContext.SetupGet(x => x.AccountSettings).Returns(accountSettings);
            var userHelper = new Mock<IUserHelper>();
            userHelper.Setup(x => x.GetUserId(It.IsAny<IPrincipal>())).Returns(String.Empty);

            var controller = new SettingsController(serviceClientMoq.Object, dbContext.Object, null, userHelper.Object);

            var stockNames = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var stockTickers = stockNames.Select(x => new StockTickerSerializable() { Name = x }).ToArray();

            serviceClientMoq.Setup(x => x.GetPricesForStocks(It.IsAny<SoapSimpleIdentity>(),
                    It.IsAny<string[]>())).
                Returns(stockTickers);


            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as StockTickerListModel;
            Assert.IsNotNull(model);

            for (int i = 0; i < stockNames.Count(); i++)
            {

                Assert.AreEqual(stockNames[i],
                    model.tickers[i].Name,
                    $"Element at index'{i}' do not match.");
                Assert.IsTrue(model.tickers[i].IsChecked);
            }
            
        }

        [TestMethod]
        public void Settings_Save_SettingsShouldBeSavedAndRedirestToHome()
        {
            const int userId = 1;
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);



            var dbContext = new Mock<ApplicationDbContext>();

            var tickerShouldBedeletedAsOldOne = new StockTickerName()
            {
                AccountSettingId = userId,
                AccountSetting = new AccountSetting() {}
            };

            var fakeStockTikers = new InMemoryDbSet<StockTickerName>() {tickerShouldBedeletedAsOldOne};

            dbContext.SetupGet(x => x.StockTickerNames).Returns(fakeStockTikers);
            StockTickerName deletedStockTicker = null;
            IEnumerable<StockTickerName> tikersShouldBeInserted = null;
            var dbHelper = new Mock<IContextDbHelper>();
            dbHelper.Setup(x => x.BulkDelete(It.IsAny<DbContext>(), It.IsAny<IEnumerable<StockTickerName>>()))
                .Callback((DbContext x, IEnumerable<StockTickerName> y) =>
                    {
                        deletedStockTicker = y.Single();
                    }
                );

            dbHelper.Setup(x => x.BulkInsert(It.IsAny<DbContext>(), It.IsAny<IEnumerable<StockTickerName>>()))
                .Callback((DbContext x, IEnumerable<StockTickerName> y) =>
                    {
                        tikersShouldBeInserted = y;
                    }
                );

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();
            var controller = new SettingsController(serviceClientMoq.Object, dbContext.Object, dbHelper.Object, null);

            var stockNames = new[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};

            controller.UserSettings = new AccountSetting()
            {
                StockTickerNames = new[] {new StockTickerName() {Name = stockNames[0]}},
                Id = userId
            };
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var tickerList = new StockTickerListModel()
            {
                tickers = new List<StockTickerModel>()
                {
                    new StockTickerModel() {IsChecked = true, Name = stockNames[0]},
                    new StockTickerModel() {IsChecked = false, Name = stockNames[1]},
                    new StockTickerModel() {IsChecked = true, Name = stockNames[2]}
                }
            };

            var result = controller.Save(tickerList) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            var shouldBeInserted = tikersShouldBeInserted as StockTickerName[] ?? tikersShouldBeInserted.ToArray();
            Assert.AreEqual(stockNames.First(), shouldBeInserted.First().Name);
            Assert.AreEqual(stockNames.Last(), shouldBeInserted.Last().Name);
            Assert.AreEqual(tickerShouldBedeletedAsOldOne, deletedStockTicker);
        }


        [TestMethod]
        public void Settings_SaveAndNoUserSettings_DeafaultSettingsShouldBeSavedAndRedirestToHome()
        {
            const int userId = 1;
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);



            var dbContext = new Mock<ApplicationDbContext>();

            var tickerShouldBedeletedAsOldOne = new StockTickerName()
            {
                AccountSettingId = userId,
                AccountSetting = new AccountSetting() { }
            };

            var fakeStockTikers = new InMemoryDbSet<StockTickerName>() { tickerShouldBedeletedAsOldOne };
            bool savedChanges = false;
            dbContext.SetupGet(x => x.StockTickerNames).Returns(fakeStockTikers);
            dbContext.Setup(x => x.SaveChanges()).Callback(() => savedChanges = true);
            StockTickerName deletedStockTicker = null;
            IEnumerable<StockTickerName> tikersShouldBeInserted = null;
            var dbHelper = new Mock<IContextDbHelper>();
            dbHelper.Setup(x => x.BulkDelete(It.IsAny<DbContext>(), It.IsAny<IEnumerable<StockTickerName>>()));

            dbHelper.Setup(x => x.BulkInsert(It.IsAny<DbContext>(), It.IsAny<IEnumerable<StockTickerName>>()));

            var serviceClientMoq = new Mock<IServiceSoapClientDecorator>();

            var accountSettings = new InMemoryDbSet<AccountSetting>();
            dbContext.SetupGet(x => x.AccountSettings).Returns(accountSettings);
            var userHelper = new Mock<IUserHelper>();
            userHelper.Setup(x => x.GetUserId(It.IsAny<IPrincipal>())).Returns(String.Empty);
            var controller = new SettingsController(serviceClientMoq.Object, dbContext.Object, dbHelper.Object, userHelper.Object);

            var stockNames = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var tickerList = new StockTickerListModel()
            {
                tickers = new List<StockTickerModel>()
                {
                    new StockTickerModel() {IsChecked = true, Name = stockNames[0]},
                    new StockTickerModel() {IsChecked = false, Name = stockNames[1]},
                    new StockTickerModel() {IsChecked = true, Name = stockNames[2]}
                }
            };

            var result = controller.Save(tickerList) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.IsTrue(savedChanges);

        }
    }
}
