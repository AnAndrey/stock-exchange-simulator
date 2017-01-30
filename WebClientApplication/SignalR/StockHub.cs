using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.SignalR
{
    [HubName("stocksHub")]

    public class StockHub : Hub
    {
        private readonly StocksPusher _stocksPusher;

        public StockHub() : this(StocksPusher.Instance) { }

        private StockHub(StocksPusher stocksPusher)
        {
            _stocksPusher = stocksPusher;
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public IEnumerable<WebClientApplication.StockServiceReference.StockTickerSerializable> getPricesForStocks()
        {
            return new StockTickerSerializable[]
            {
                new StockTickerSerializable() {Name = "blabla", Price = 1111},
                new StockTickerSerializable() {Name = "qweqwe", Price = 3333}
            };
        }
    }
}