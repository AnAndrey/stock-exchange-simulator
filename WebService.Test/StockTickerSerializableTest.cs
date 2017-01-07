using System;
using System.CodeDom;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebService.Test
{
    /// <summary>
    /// Summary description for StockTickerSerializableTest
    /// </summary>
    [TestClass]
    public class StockTickerSerializableTest
    {
        private const string CTestName = "327A7812-47E1-4F56-9612-999FF60D1F09";
        private const int CTestPrice = 111;
        [TestMethod]
        public void Equals_PassNotStockTickerSerializable_False()
        {
            var stock = new StockTickerSerializable();
            Assert.AreNotEqual( stock, new object());
        }

        [TestMethod]
        public void Equals_DifferentNames_False()
        {
            var stock = new StockTickerSerializable() {Name = CTestName + CTestName };
            Assert.AreNotEqual(stock, new StockTickerSerializable() {Name = CTestName } );
        }

        [TestMethod]
        public void Equals_SameNames_True()
        {
            var stock = new StockTickerSerializable() { Name = CTestName };
            Assert.AreEqual(stock, new StockTickerSerializable() { Name = CTestName });
        }

        [TestMethod]
        public void Price_SetPrice_ActualPrice()
        {
            var stock = new StockTickerSerializable() { Price = CTestPrice };
            Assert.AreEqual(CTestPrice, stock.Price);
        }
    }
}
