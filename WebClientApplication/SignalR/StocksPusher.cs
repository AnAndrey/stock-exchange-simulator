using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WebClientApplication.Api;
using WebClientApplication.Controllers;
using WebClientApplication.StockServiceReference;
using static WebClientApplication.StockServiceReference.SoapSimpleIdentity;
namespace WebClientApplication.SignalR
{
    public class StocksPusher
    {
        private static StocksPusher _instance;

        private IUserSettingsProvider _userSettingsProvider;
        private IServiceSoapClientDecorator _soapClient;


        private StocksPusher(IHubContext hubContext, IUserSettingsProvider userSettingsProvider, IServiceSoapClientDecorator soapClient)
        {
            HubContext = hubContext;
            _userSettingsProvider = userSettingsProvider;
            _soapClient = soapClient;

            HubContext.Groups.Add("47f6d54c-2804-4672-a17f-c11f4081e50f", "47f6d54c-2804-4672-a17f-c11f4081e50f");



        }

        public static StocksPusher GetInstance(IUserSettingsProvider userSettingsProvider, IServiceSoapClientDecorator soapClient)
        {
                return _instance = new StocksPusher(GlobalHost.ConnectionManager.GetHubContext<StockHub>(),
                                                    userSettingsProvider,
                                                    soapClient);
        }

        private IHubContext HubContext{ get;set;}


        public  void PushStocks()
        {

            var stocks = _soapClient.GetPricesForStocks(TheSimplestIdentityEver, null);

            foreach (var user in StockHub.MyUsers)
            {
                var tickers =_userSettingsProvider.GetStockTickerNames(user.Value);
                if (tickers != null)
                {
                    var setOfTickers = new HashSet<string>(tickers);
                    var customezedStocks = stocks.Where(s => setOfTickers.Contains(s.Name));
                    HubContext.Clients.All.updateStocks(customezedStocks);
                }
            }
            
            //HubContext.All.updateStocks(stocks);
            
            //HubContext.Clients.Client("47f6d54c-2804-4672-a17f-c11f4081e50f").updateStocks(stocks);
        }
    }
}