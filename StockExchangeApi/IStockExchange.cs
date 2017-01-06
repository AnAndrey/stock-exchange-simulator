using System.Collections.Generic;

namespace StockExchangeApi
{
    public interface IStockExchange
    {
        IEnumerable<IStockTicker> GetAllStockTickers();
        IEnumerable<IStockTicker> GetStockTickers(IEnumerable<string> name);
        IStockTicker GetStockTicker(string name);
    }
}
