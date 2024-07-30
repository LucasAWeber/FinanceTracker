using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinanceTrackerApp.Models
{
    public partial class Investment : ObservableObject
    {
        [ObservableProperty]
        [Index(0)]
        private string _name = "";
        [ObservableProperty]
        [Index(1)]
        private string _symbol = "";
        [ObservableProperty]
        [Index(2)]
        private int _shares = 1;
        [ObservableProperty]
        [Index(3)]
        private double _value = 0;
        [ObservableProperty]
        [Index(4)]
        private double _total = 0;
        [ObservableProperty]
        [Index(5)]
        private InvestmentType _type = InvestmentType.other;
        [ObservableProperty]
        [Index(6)]
        private StockExchange _stockExchange = StockExchange.none;

        public async Task UpdateInvestment()
        {
            if (StockExchange == StockExchange.NYSE)
            {
                try
                {
                    var data = await Yahoo.GetHistoricalAsync(Symbol, DateTime.Now, DateTime.Now);
                    Value = (double?)data.LastOrDefault()?.Close ?? Value;
                }
                catch
                {
                    Debug.WriteLine("Invalid symbol");
                    Value = 0;
                }
            }
            
            Total = Shares * Value;
        }
    }

    public sealed class InvestmentMap : ClassMap<Investment>
    {
        public InvestmentMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Symbol).Index(1);
            Map(m => m.Shares).Index(2);
            Map(m => m.Value).Index(3);
            Map(m => m.Total).Index(4);
            Map(m => m.Type).Index(5);
            Map(m => m.StockExchange).Index(6);
        }
    }
}
