using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClinetApplication.StockServiceReference;
using CommonSecurity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebClinetApplication.Models;
using static WebClinetApplication.StockServiceReference.TheSimplestIdentity;

namespace WebClinetApplication.Controllers
{
    [HandleError(ExceptionType = typeof(System.Exception), View = "Exception")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController()
        {
            _dbContext = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }
        
        //
        // GET: /Home/Index
        public ActionResult Index()
        {
            return View();
        }

        private AccountSetting UserSettings
        {
            get
            {
                var userId = User.Identity.GetUserId();
                return _dbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);
            }
        }

        protected virtual WebServiceSoapClient WebServiceSoapClient { get; } = new WebServiceSoapClient();

        //
        // GET: /Home/StockPrices
        public ActionResult StockPrices()
        {
            try
            {
                string[] tickerNames = null;
                if (Request.IsAuthenticated)
                {
                    var userSetting = UserSettings;
                    if (userSetting != null)
                        tickerNames = userSetting.StockTickerNames.Select(x => x.Name).ToArray();
                }

                var tickers = WebServiceSoapClient.GetPricesForStocks(TheSimplestIdentityEver, tickerNames);

                return PartialView("_StockPricesView", tickers);
            }
            catch (Exception ex)
            {
                return View("Exception", new HandleErrorInfo(ex, "Home", "StockPrices") );
            }
        }
    }
}


