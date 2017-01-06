using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClinetApplication.StockServiceReference;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using CommonSecurity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebClinetApplication.Models;
using WebGrease.Css.Extensions;

namespace WebClinetApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
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


        public ActionResult Settings()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account",new { returnUrl= Url.Action("Settings", "Home")});
            }

            HashSet<string> userStocks = null;

            using (var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>())
            {
                var userSetting = UserSettings;
                if (userSetting != null)
                {
                    userStocks = new HashSet<string>();
                    userSetting.StockTickerNames.All(x => userStocks.Add(x.Name));
                }

            }
            
            var tickers = WebServiceSoapClient.GetPricesForStocks(TheSimplestIdentityEver, null);
            var tickersModels = tickers.Select(t =>  new StockTickerModel()
            {
                Name = t.Name,
                Price = t.Price,
                IsChecked = userStocks?.Contains(t.Name) ?? true
            }).ToList();

            StockTickerModelList list = new StockTickerModelList() {tickers = tickersModels};
            return View(list);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Savev(StockTickerModelList incomeTickers)
        {
            
            using (var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>())
            {

                var tickerNames = incomeTickers.tickers.Where(x => x.IsChecked).Select(x =>
                    new StockTickerName()
                    {
                        Name = x.Name,

                    }).ToList();
                
                var userSetting = UserSettings;
                // Add user.AccountSetting
                if (userSetting == null)
                {
                    userSetting = _dbContext.AccountSettings.Add(new AccountSetting()
                    {
                        ApplicationUserId = User.Identity.GetUserId(),
                    });
                    _dbContext.SaveChanges();
                }

                var oldTickers = _dbContext.StockTickerNames.Where(x => x.AccountSettingId == userSetting.Id);
                

                tickerNames.ForEach(x => x.AccountSettingId = userSetting.Id);
                _dbContext.BulkDelete(oldTickers);
                _dbContext.BulkInsert(tickerNames);

            }

            return RedirectToAction("Index", "Home");
        }

        private StockServiceReference.TheSimplestIdentity _theSimplestIdentity;
        private StockServiceReference.TheSimplestIdentity TheSimplestIdentityEver
        {
            get
            {
                if (_theSimplestIdentity == null)
                {
                    var svc = new WebServiceSoapClient();
                    _theSimplestIdentity = new StockServiceReference.TheSimplestIdentity()
                    {
                        Token = TheSimplestIdentityValidator.CreateTrustFullIdentity().Token
                    };
                }
                return _theSimplestIdentity;
            }
        }

        private WebServiceSoapClient _webServiceSoapClient;
        public WebServiceSoapClient WebServiceSoapClient => _webServiceSoapClient ?? (_webServiceSoapClient = new WebServiceSoapClient());

        public ActionResult StockPrices()
        {
                string[] tickerNames = null;
                if (Request.IsAuthenticated)
                {
                    using (var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>())
                    {
                        var userSetting = UserSettings;
                        if (userSetting != null)
                            tickerNames = userSetting.StockTickerNames.Select(x => x.Name).ToArray();
                    }
                }

                var tickers = WebServiceSoapClient.GetPricesForStocks(TheSimplestIdentityEver, tickerNames);
                return PartialView("_StockPricesView", tickers);

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult()
            {
                ViewName = "Exception",
                ViewData = new ViewDataDictionary(filterContext.Exception.InnerException ?? filterContext.Exception)
            };

        }
    }
}


