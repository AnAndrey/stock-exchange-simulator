using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using WebClientApplication.StockServiceReference;

namespace WebClientApplication.SignalR
{
    [HubName("stocksHub")]

    public class StockHub : Hub
    {

        public static ConcurrentDictionary<string, string> MyUsers = new ConcurrentDictionary<string, string>();

        public StockHub()
        {

        } //: this(StocksPusher.Instance) { }


        public override Task OnConnected()
        {
            MyUsers.TryAdd(Context.ConnectionId,  Context.Request.User.Identity.GetUserId() );
            var r = Context.User.Identity.GetUserId();
            return base.OnConnected();
        }
        //private StockHub(StocksPusher stocksPusher)
        //{
        //    _stocksPusher = stocksPusher;
        //}
    }
}