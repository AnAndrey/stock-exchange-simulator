using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WebClientApplication.StockServiceReference;
using Microsoft.AspNet.Identity;
using WebClientApplication.Api;
using WebClientApplication.Models;
using WebClientApplication.SignalR;
using static WebClientApplication.StockServiceReference.SoapSimpleIdentity;

namespace WebClientApplication.Controllers
{
    [HandleError(ExceptionType = typeof(System.Exception), View = "Exception")]
    public class HomeController : Controller
    {

        private static StocksPusher _stocksPusher;

        private static Timer _timer;
        

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);


        public HomeController()
        {
            var dbContext = new ApplicationDbContext();

            Setup(new ServiceSoapClient(new WebServiceSoapClient()), 
                dbContext, 
                new UserHelper(), 
                new UserSettingsProvider(dbContext));
            
        }

        private void PushStocks(object state)
        {
            if (_stocksPusher!=null)
            {
                _stocksPusher.PushStocks();
            }
        }

        public HomeController(IServiceSoapClientDecorator webService, ApplicationDbContext dbContext, IUserHelper userHelper, IUserSettingsProvider userSettingsProvider)
        {
            Setup(webService, dbContext, userHelper, userSettingsProvider);
        }

        private void Setup(IServiceSoapClientDecorator webService, ApplicationDbContext dbContext, IUserHelper userHelper, IUserSettingsProvider userSettingsProvider)
        {
            WebServiceSoapClient = webService;
            UserHelper = userHelper;
            DbContext = dbContext;
            UserSettingsProvider = userSettingsProvider;
            _stocksPusher = StocksPusher.GetInstance(userSettingsProvider, webService);

            _timer = new Timer(PushStocks, null, _updateInterval, _updateInterval);
        }

        protected override void Dispose(bool disposing)
        {
            DbContext.Dispose();
        }

        //
        // GET: /Home/Index
        public ActionResult Index()
        {
            return View("Index");
        }

        private IUserHelper UserHelper { get; set; }
        private IUserSettingsProvider UserSettingsProvider { get; set; }

        private ApplicationDbContext DbContext { get; set; }


        //private AccountSetting _userSettings;
        //public AccountSetting UserSettings
        //{
        //    get
        //    {
        //        if (_userSettings == null)
        //        {
        //            var userId = UserHelper.GetUserId(User);
        //            _userSettings = DbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);

        //        }
        //        return _userSettings;
        //    }
        //    set {_userSettings = value;}
        //}

        private IServiceSoapClientDecorator WebServiceSoapClient { get; set; }

        //
        // GET: /Home/StockPrices
        public ActionResult StockPrices()
        {
            try
            {
                string[] tickerNames = null;
                if (Request.IsAuthenticated)
                {
                    tickerNames = UserSettingsProvider.GetStockTickerNames(UserHelper.GetUserId(User));
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

    public interface IUserSettingsProvider
    {
        AccountSetting GetAccountSetting(string userId);
        string[] GetStockTickerNames(string userId);
    }

    public class UserSettingsProvider: IUserSettingsProvider
    {
        private readonly ApplicationDbContext DbContext;

        public UserSettingsProvider(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public AccountSetting GetAccountSetting(string userId)
        {
            var dbContext = new ApplicationDbContext();
            {

                var userSettings = dbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);
                return userSettings;
            }
        }

        public string[] GetStockTickerNames(string userId)
        {
            var userSettings = GetAccountSetting(userId);
            return userSettings?.StockTickerNames.Select(x => x.Name).ToArray();
        }

    }
}


