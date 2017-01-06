using System.Collections.Generic;

namespace StockExchangeApi
{
    /// <summary>
    /// Interface of stock exchange
    /// </summary>
    public interface IStockExchange
    {
        /// <summary>
        /// Returns stock prices for all possible list of stocks.
        /// </summary>
        IEnumerable<IStockTicker> GetAllStockTickers();
        
        /// <summary>
        /// Returns stock prices for particular list of stock names.
        /// </summary>
        IEnumerable<IStockTicker> GetStockTickers(IEnumerable<string> name);

        /// <summary>
        /// Returns stock price for a particular stock name.
        /// </summary>
        IStockTicker GetStockTicker(string name);
    }
}
