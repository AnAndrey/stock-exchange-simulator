using System;

namespace StockExchangeApi
{
    public interface IStockTicker
    {
        string Name { get; set; }
        int Price { get; set; }
    }
}