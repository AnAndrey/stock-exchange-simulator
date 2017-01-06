using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using static WebClinetApplication.StockServiceReference.TheSimplestIdentity;

using WebClinetApplication.Models;
using WebClinetApplication.StockServiceReference;

namespace WebClinetApplication.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SettingsController()
        {
             _dbContext = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }
        private AccountSetting UserSettings
        {
            get
            {
                var userId = User.Identity.GetUserId();
                return _dbContext.AccountSettings.SingleOrDefault(x => x.ApplicationUserId == userId);
            }
        }

        //
        // GET: /Settings/Index
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Settings") });
            }

            HashSet<string> userStocks = null;

            var userSetting = UserSettings;
            if (userSetting != null)
            {
                userStocks = new HashSet<string>();
                userSetting.StockTickerNames.All(x => userStocks.Add(x.Name));
            }

            var tickers = WebServiceSoapClient.GetPricesForStocks(TheSimplestIdentityEver, null);
            var tickersModels = tickers.Select(t => new StockTickerModel()
            {
                Name = t.Name,
                Price = t.Price,
                IsChecked = userStocks?.Contains(t.Name) ?? true
            }).ToList();

            var listOfTickers = new StockTickerListModel() { tickers = tickersModels };
            return View(listOfTickers);
        }

        //
        // Post: /Settings/Save
        [HttpPost]
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
                _dbContext.BulkDelete(oldTickers);
                _dbContext.BulkInsert(tickerNames);

            return RedirectToAction("Index", "Home");
        }

        private AccountSetting AddDefaultUserSettings()
        {
            var userSettings = _dbContext.AccountSettings.Add(new AccountSetting()
            {
                ApplicationUserId = User.Identity.GetUserId(),
            });
            _dbContext.SaveChanges();

            return userSettings;
        }

        public virtual WebServiceSoapClient WebServiceSoapClient { get;} = new WebServiceSoapClient();
    }
}