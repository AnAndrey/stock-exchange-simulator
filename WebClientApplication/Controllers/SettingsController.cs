using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebClientApplication.Api;
using static WebClientApplication.StockServiceReference.SoapSimpleIdentity;
using WebClientApplication.Models;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SettingsController()
        {
             _dbContext = new ApplicationDbContext();
            UserHelper = new UserHelper();
            DbHelper = new DbContextHelper();
        }

        public SettingsController(IServiceSoapClientDecorator webService, 
            ApplicationDbContext dbContext, 
            IContextDbHelper dbHelper,
            IUserHelper userHelper) 
        {
            _webService = webService;
            _dbContext = dbContext;
            DbHelper = dbHelper;
            UserHelper = userHelper;
        }
        private IUserHelper UserHelper { get; }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }
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
            set { _userSettings = value; }
        }

        //
        // GET: /Settings/Index
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Settings") });
            }

            HashSet<string> enabledUserStocks = null;

            var userSetting = UserSettings;
            if (userSetting != null)
            {
                enabledUserStocks = new HashSet<string>();
                userSetting.StockTickerNames.All(x => enabledUserStocks.Add(x.Name));
            }

            var tickers = WebServiceSoapClient.GetPricesForStocks(TheSimplestIdentityEver, null);
            var tickersModels = tickers.Select(t => new StockTickerModel()
            {
                Name = t.Name,
                Price = t.Price,
                IsChecked = enabledUserStocks?.Contains(t.Name) ?? true
            }).ToList();

            var listOfTickers = new StockTickerListModel() { tickers = tickersModels };
            return View(listOfTickers);
        }

        //
        // Post: /Settings/Save
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Save(StockTickerListModel incomeTickers)
        {

            var tickerNames = incomeTickers.tickers.Where(x => x.IsChecked).Select(x =>
                new StockTickerName()
                {
                    Name = x.Name,

                }).ToList();

            var userSetting = UserSettings ?? AddDefaultUserSettings();

            var oldTickers = _dbContext.StockTickerNames.Where(x => x.AccountSettingId == userSetting.Id);

            tickerNames.ForEach(x => x.AccountSettingId = userSetting.Id);
            DbHelper.BulkDelete(_dbContext, oldTickers);
            DbHelper.BulkInsert(_dbContext, tickerNames);

            return RedirectToAction("Index", "Home");
        }

        private IContextDbHelper DbHelper { get; } = new DbContextHelper();


        private AccountSetting AddDefaultUserSettings()
        {
            var userSettings = _dbContext.AccountSettings.Add(new AccountSetting()
            {
                ApplicationUserId = UserHelper.GetUserId(User),
            });
            _dbContext.SaveChanges();

            return userSettings;
        }

        private IServiceSoapClientDecorator _webService;

        protected virtual IServiceSoapClientDecorator WebServiceSoapClient
        {
            get
            {
                if (_webService == null)
                    _webService = new ServiceSoapClient(new WebServiceSoapClient());

                return _webService;
            }
        }
    }
}