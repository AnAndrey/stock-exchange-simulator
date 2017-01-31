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


        public StockHub()
        {
        } //: this(StocksPusher.Instance) { }

        //private StockHub(StocksPusher stocksPusher)
        //{
        //    _stocksPusher = stocksPusher;
        //}
    }
}