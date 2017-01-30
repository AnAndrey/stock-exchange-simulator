using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.SignalR
{
    public class StocksPusher
    {
        private readonly static Lazy<StocksPusher> _instance =
            new Lazy<StocksPusher>(
                () => new StocksPusher(GlobalHost.ConnectionManager.GetHubContext<StockHub>().Clients));

        private readonly Timer _timer;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);


        private StocksPusher(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _timer = new Timer(PushStocks, null, _updateInterval, _updateInterval);

        }

        public static StocksPusher Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private void PushStocks(object state)
        {
           var stocks =  new StockTickerSerializable[]
           {
                new StockTickerSerializable() {Name = "blabla", Price = 1111},
                new StockTickerSerializable() {Name = "qweqwe", Price = 3333}
           };
           Clients.All.updateStocks(stocks);
        }
    }
}