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
        private readonly ApplicationDbContext _dbContext;

        private readonly StocksPusher _stocksPusher = StocksPusher.Instance;


        private readonly Timer _timer;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);

        public HomeController()
        {
            _dbContext = new ApplicationDbContext();
            WebServiceSoapClient = new ServiceSoapClient(new WebServiceSoapClient());
            UserHelper = new UserHelper();
            _timer = new Timer(_stocksPusher.PushStocks, null, _updateInterval, _updateInterval);

        }

 

        public HomeController(IServiceSoapClientDecorator webService, ApplicationDbContext dbContext, IUserHelper userHelper)
        {
            WebServiceSoapClient = webService;
            UserHelper = userHelper;
            _dbContext = dbContext;
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

        private IUserHelper UserHelper { get; }
        private AccountSetting _userSettings;
        public AccountSetting UserSettings
        {
            get
            {
                if (_userSettings == null)
                {
                    var userId = UserHelper.GetUserId(User);
                    _userSettings = _dbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);
                    
                }
                return _userSettings;
            }
            set {_userSettings = value;}
        }

        private IServiceSoapClientDecorator WebServiceSoapClient { get; }

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


