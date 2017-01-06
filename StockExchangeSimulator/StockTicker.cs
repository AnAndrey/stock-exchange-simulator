using StockExchangeApi;

namespace StockExchangeSimulator
{
    public class StockTicker:IStockTicker
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
}