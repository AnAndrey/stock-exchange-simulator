using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace StockExWebService
{
    /// <summary>
    /// Summary description for StockExWebService
    /// </summary>
    [WebService(Namespace = "http://ang-csharp-assignment.edu/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class StockExWebService : System.Web.Services.WebService
    {

        public AuthHeader Credentials;

        //[SoapHeader("Credentials")]
        //[WebMethod]
        //public List<StockTicker> GetPricesForStocks(List<string> stockSymbolList)
        //{
        //    // Fail the call if the caller is not authorized 
        //    if (Credentials.UserName.ToLower () != "jeff" || Credentials.Password.ToLower () != "imbatman")
        //        throw new SoapException ("Unauthorized", SoapException.ClientFaultCode);


        //    //Check on count
        //    var tickers = new List<StockTicker>(stockSymbolList.Count);

        //    Random random = new Random();
        //    tickers.AddRange(stockSymbolList.Select(symbol => new StockTicker()
        //        {
        //            Symbol = symbol,
        //            Price = random.Next(c_priceMin, c_priceMax)
        //        }));

        //    return tickers;
        //}

        [WebMethod]
        public int GetPricesForStocks(int t)
        {
            return t;
        }
    }

    public class AuthHeader : SoapHeader
    {
        public string UserName;
        public string Password;
    }
}
