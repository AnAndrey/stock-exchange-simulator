using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.Api
{
    public interface IServiceSoapClientDecorator
    {
        StockTickerSerializable[] GetPricesForStocks(SoapSimpleIdentity soapSimpleIdentity, string[] stockSymbolList);
    }

    public class ServiceSoapClient : IServiceSoapClientDecorator
    {
        private readonly WebServiceSoapClient _client;
        public ServiceSoapClient(WebServiceSoapClient client)
        {
            _client = client;
        }

        public StockTickerSerializable[] GetPricesForStocks(SoapSimpleIdentity soapSimpleIdentity, string[] stockSymbolList)
        {
            return _client.GetPricesForStocks(soapSimpleIdentity, stockSymbolList);
        }
    }
}
