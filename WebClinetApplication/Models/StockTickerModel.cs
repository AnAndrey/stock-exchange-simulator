using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebClinetApplication.Models
{
    public class StockTickerModel
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public bool IsChecked { get; set; }
    }

    public class StockTickerModelList
    {
        public List<StockTickerModel> tickers { get; set; }
    }
}