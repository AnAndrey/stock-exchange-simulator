using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Services;
using System.Web.Services.Protocols;
using CommonSecurity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using StockExchangeApi;

namespace WebService
{
    /// <summary>
    /// ASMX web service to simulate a stock exchange.
    /// </summary>
    [WebService(Namespace = "https://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WebService : System.Web.Services.WebService
    {
        public TheSimplestIdentity Identity;
        protected TheSimplestIdentityValidator Authenticator { get; set; } =new TheSimplestIdentityValidator();


        /// <summary>
        /// Method returns stock prices for a list of stock codes.
        /// </summary>
        [WebMethod]
        [SoapHeader("Identity")]
        public List<StockTickerSerializable> GetPricesForStocks(List<string> stockSymbolList)
        {

            var container = new UnityContainer().LoadConfiguration();
            var stockEx = container.Resolve<IStockExchange>();


            if (!Authenticator.CanWeTrustTo(Identity))
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }

            var tickers = stockEx.GetStockTickers(stockSymbolList);

            return tickers.Select(x => new StockTickerSerializable()
            {
                Name = x.Name, Price = x.Price
                
            }).ToList();

        }
    }

}
