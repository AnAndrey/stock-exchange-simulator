using System;

namespace WebService
{
    /// <summary>
    /// Serializable object that contains Stock Name and Stock Price
    /// </summary>
    [Serializable]
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class StockTickerSerializable
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public override bool Equals(object obj)
        {
            var a = obj as StockTickerSerializable;
            return a != null && Name.Equals(a.Name, StringComparison.Ordinal);
        }
    }
}