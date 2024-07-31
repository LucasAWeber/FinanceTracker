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
        private float _shares = 1;
        [ObservableProperty]
        [Index(3)]
        private float _value = 0;
        [ObservableProperty]
        [Index(4)]
        private float _total = 0;
        [ObservableProperty]
        [Index(5)]
        private InvestmentType _type = InvestmentType.other;
        [ObservableProperty]
        [Index(7)]
        private StockExchange _stockExchange = StockExchange.none;

        public async Task UpdateInvestment()
        {
            float conversion = 1;
            string symbol = Symbol;
            if (!string.IsNullOrWhiteSpace(symbol) && StockExchange != StockExchange.none && Type != InvestmentType.other && Type != InvestmentType.Cash)
            {
                if (StockExchange == StockExchange.TSX)
                {
                    symbol += ".TO";
                } else if (StockExchange == StockExchange.NYSE)
                {
                    // Change how this conversion is calculated and possibly add seperate field for currency type
                    conversion = 1.35f;
                }
                try
                {
                    // market closes 4:00pm
                    TimeOnly marketClose = new(16, 0);
                    // Use yesterday as most recent (as to avoid constantly updating prices)
                    DateTime yesterday = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)).ToDateTime(marketClose);
                    // Used to get the day range from 'dayBuffer' to Yesterday incase the market was closed past few days (accounts for weekends and weekday holidays)
                    DateTime dayBuffer = yesterday.AddDays(-4);
                    
                    var data = await Yahoo.GetHistoricalAsync(symbol, dayBuffer, yesterday);
                    Value = conversion * (float?)data.LastOrDefault()?.Close ?? Value;
                }
                catch
                {
                    Debug.WriteLine($"Invalid symbol {symbol}");
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
