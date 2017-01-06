using System;

namespace WebService
{
    [Serializable]
    public class StockTickerSerializable
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
}