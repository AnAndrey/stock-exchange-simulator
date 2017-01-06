using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.Configuration;
using CommonSecurity;
using StockExchangeApi;
using StockExchangeSimulator;

namespace WebService
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "https://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 

    public class WebService : System.Web.Services.WebService
    {
        public TheSimplestIdentity Identity;
        protected TheSimplestIdentityValidator Authenticator { get; set; } =new TheSimplestIdentityValidator();

        [WebMethod]
        [SoapHeader("Identity")]
        public List<StockTickerSerializable> GetPricesForStocks(List<string> stockSymbolList)
        {
           // if (!Authenticator.CanWeTrustTo(Identity))
            {
                //Context.Response.Status = "403 Forbidden";
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
            IStockExchange stockEx = new StockExchange();

            var tickers = stockEx.GetStockTickers(stockSymbolList);

            return tickers.Select(x => new StockTickerSerializable()
            {
                Name = x.Name, Price = x.Price
                
            }).ToList();

        }
    }

}
