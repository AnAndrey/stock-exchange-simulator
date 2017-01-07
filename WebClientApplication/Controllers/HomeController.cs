using System;
using System.Linq;
using System.Web.Mvc;
using WebClientApplication.StockServiceReference;
using Microsoft.AspNet.Identity;
using WebClientApplication.Api;
using WebClientApplication.Models;
using static WebClientApplication.StockServiceReference.SoapSimpleIdentity;

namespace WebClientApplication.Controllers
{
    [HandleError(ExceptionType = typeof(System.Exception), View = "Exception")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController()
        {
            _dbContext = new ApplicationDbContext();

        }

        public HomeController(IServiceSoapClientDecorator webService) : this()
        {
            _webService = webService;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        //
        // GET: /Home/Index
        public ActionResult Index()
        {
            return View("Index");
        }

        private AccountSetting _userSettings;
        public AccountSetting UserSettings
        {
            get
            {
                if (_userSettings == null)
                {
                    var userId = User.Identity.GetUserId();
                    _userSettings = _dbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);
                    
                }
                return _userSettings;
            }
            set {_userSettings = value;}
        }

        private IServiceSoapClientDecorator _webService;
        protected virtual IServiceSoapClientDecorator WebServiceSoapClient
        {
            get
            {
                if(_webService == null)
                    _webService = new ServiceSoapClient( new WebServiceSoapClient());

                return _webService;
            }
        }

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

                var tickers = WebServiceSoapClient.GetPricesForStocks( TheSimplestIdentityEver, tickerNames);

                return PartialView("_StockPricesView", tickers);
            }
            catch (Exception ex)
            {
                return View("Exception", new HandleErrorInfo(ex, "Home", "StockPrices") );
            }
        }
    }
}


