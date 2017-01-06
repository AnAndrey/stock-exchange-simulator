using System;
using StockExchangeApi;

namespace StockExchangeSimulator
{
    [Serializable]
    public class StockTickerSerializable
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
}