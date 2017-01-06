using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockExWebService
{
    [Serializable]
    public class StockTicker
    {
        public string Symbol { get; set; }
        public int Price { get; set; }
    }
}