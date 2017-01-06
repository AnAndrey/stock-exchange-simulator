using System;

namespace WebService
{
    /// <summary>
    /// Serializable object that contains Stock Name and Stock Price
    /// </summary>
    [Serializable]
    public class StockTickerSerializable
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
}