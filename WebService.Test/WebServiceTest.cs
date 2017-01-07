using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using CommonSecurity;
using Moq;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using Castle.Components.DictionaryAdapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockExchangeSimulator;
using WebService;

namespace NUnit.WebService
{
    [TestClass]
    public class WebServiceTest
    {
        private readonly string [] _stocks = new[]
{
                "Exxon Mobil Corporation", "General Electric Company", "Microsoft Corporation", "BP p.l.c.",
                "Citigroup Inc.", "Procter & Gamble Company (The)", "Wal-Mart Stores Inc.", "Pfizer Inc.",
                "HSBC Holdings plc.", "Toyota Motor Corporation", "JOHNSON & JOHNSON", "Bank of America Corporation",
                "American International Group Inc.", "TotalFinaElf S.A.", "Novartis AG", "Altria Group",
                "GLAXOSMITHKLINE PLC", "Mitsubishi UFJ Financial Group Inc", "J.P.Morgan Chase & Co.",
                "ROYAL DUTCH SHELL PLC", "ChevronTexaco Corporation", "Sanofi-Aventis SA",
                "Vodafone AirTouch Public Limited Company", "Intel Corporation",
                "International Business Machines Corporation", "ENI S.p.A.", "Cisco Systems Inc.",
                "Berkshire Hathaway Inc.", "UBS AG", "Wells Fargo Cap IX", "AT&T Inc.",
                "Coca-Cola Company (The)", "China Mobile(Hong Kong) Ltd.", "Pepsico Inc.",
                "Verizon Communications Inc.", "CONOCOPHILLIPS", "Genentech Inc.", "Amgen Inc.",
                "Banco Santander Central Hispano S.A.", "Hewlett-Packard Company", "Google Inc."
            };

        [TestMethod]
        public void GetPricesForStocks_NotAuthorized_NoTickersAndAccessDenied()
        {
            // TODO: Add your test code here

            var w = new global::WebService.WebService {Identity = new SoapSimpleIdentity()};

            HttpContext.Current = FakeHttpContext();

            var tickers = w.GetPricesForStocks(null);
            Assert.IsNull(tickers);
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, HttpContext.Current.Response.StatusCode);
        }
        
        [TestMethod]
        public void GetPricesForStocks_Authorized_ListOfAllTickers()
        {
            // TODO: Add your test code here

            var w = new global::WebService.WebService { Identity = new SoapSimpleIdentity( TheSimplestIdentityValidator.CreateTrustFullIdentity()) };

            HttpContext.Current = FakeHttpContext();
            
            var tickers = w.GetPricesForStocks(null);
            var expectedList = GenerateListOfAllStocktickers();
            CollectionAssert.AreEqual(expectedList, tickers );
        }

        [TestMethod]
        public void GetPricesForStocks_IncomeCustomList_ListOfCustomTickers()
        {
            // TODO: Add your test code here

            var w = new global::WebService.WebService { Identity = new SoapSimpleIdentity(TheSimplestIdentityValidator.CreateTrustFullIdentity()) };

            HttpContext.Current = FakeHttpContext();

            var stockName = StockExchange.DefaultStocks.First();

            var tickers = w.GetPricesForStocks(new List<string>() {stockName});

            var expectedList = new [] { new StockTickerSerializable() {Name = stockName} };

            Assert.IsTrue(tickers.TrueForAll(x => x.Price != 0));
            CollectionAssert.AreEqual(expectedList, tickers);
        }

        [TestMethod]
        public void GetPricesForStocks_IncomeEmptyList_ListOfAllTickers()
        {
            // TODO: Add your test code here

            var w = new global::WebService.WebService { Identity = new SoapSimpleIdentity(TheSimplestIdentityValidator.CreateTrustFullIdentity()) };

            HttpContext.Current = FakeHttpContext();

            var tickers = w.GetPricesForStocks(new List<string>());

            var expectedList = GenerateListOfAllStocktickers();

            CollectionAssert.AreEqual(expectedList, tickers);
        }

        private static List<StockTickerSerializable> GenerateListOfAllStocktickers()
        {
            return StockExchange.DefaultStocks.Select(x => new StockTickerSerializable(){Name = x}).ToList();
        }

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://app.crossover.com/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }


    }

}
