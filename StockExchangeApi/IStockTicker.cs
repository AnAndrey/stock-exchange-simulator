using System;

namespace StockExchangeApi
{
    /// <summary>
    /// Interface of stock ticker
    /// </summary>
    public interface IStockTicker
    {
        string Name { get; set; }
        int Price { get; set; }
    }
}