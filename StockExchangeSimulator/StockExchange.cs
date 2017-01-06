using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockExchangeApi;

namespace StockExchangeSimulator
{
    public class StockExchange: IStockExchange
    {
        private const int CPriceMin = 1;
        private const int CPriceMax = 1000;
        Random _randomizer = new Random();

        readonly HashSet<string> _defaultStocks = new HashSet<string>()
            {
                "Exxon Mobil Corporation", "General Electric Company", "Microsoft Corporation", "BP p.l.c.",
                "Citigroup Inc.", "Procter & Gamble Company (The)", "Wal-Mart Stores Inc.", "Pfizer Inc.",
                "HSBC Holdings plc.", "Toyota Motor Corporation", "JOHNSON & JOHNSON", "Bank of America Corporation",
                "American International Group Inc.", "TotalFinaElf S.A.", "Novartis AG", "Altria Group",
                "GLAXOSMITHKLINE PLC", "Mitsubishi UFJ Financial Group Inc", "J.P.Morgan Chase & Co.",
                "ROYAL DUTCH SHELL PLC", "ChevronTexaco Corporation", "Sanofi-Aventis SA",
                "Vodafone AirTouch Public Limited Company", "Intel Corporation",
                "International Business Machines Corporation", "ENI S.p.A.", "Cisco Systems Inc.",
                "Berkshire Hathaway Inc.", "UBS AG", "Wells Fargo Cap IX", "AT&T Inc.", "ROYAL DUTCH SHELL PLC",
                "Coca-Cola Company (The)", "China Mobile(Hong Kong) Ltd.", "Pepsico Inc.",
                "Verizon Communications Inc.", "CONOCOPHILLIPS", "Genentech Inc.", "Amgen Inc.",
                "Banco Santander Central Hispano S.A.", "Hewlett-Packard Company", "Google Inc."
            };

        public virtual IStockTicker GetStockTicker(string name)
        {
            
            return new StockTicker()
            {
                Name = name,
                Price = _randomizer.Next(CPriceMin, CPriceMax)
            };

        }

        public virtual IEnumerable<IStockTicker> GetAllStockTickers()
        {
            return _defaultStocks.Select(GetStockTicker).ToList();
        }

        public virtual IEnumerable<IStockTicker> GetStockTickers(IEnumerable<string> names)
        {
            if (names != null )
            {
                var enumerableNames = names.ToArray();
                if(enumerableNames.Any())
                    return enumerableNames.Where(x => _defaultStocks.Contains(x)).
                         Select(GetStockTicker).ToList();
            }

            return GetAllStockTickers();
            
        }
    }
}
