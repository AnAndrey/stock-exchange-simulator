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



        private StocksPusher(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

           

        }

        public static StocksPusher Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients{get;set;}

        public void PushStocks(object state)
        {
        
           var stocks =  new StockTickerSerializable[]
           {
                new StockTickerSerializable() {Name = "Microsoft Corporation", Price =  new Random((int)Math.Floor(1112.11)).Next()},
        
                new StockTickerSerializable() {Name = "qweqwe", Price = 3333}
           };
           Clients.All.updateStocks(stocks);
        }

        public void PushStocks(IEnumerable<StockTickerSerializable> stocks)
        {
            Clients.All.updateStocks(stocks);
        }
    }
}