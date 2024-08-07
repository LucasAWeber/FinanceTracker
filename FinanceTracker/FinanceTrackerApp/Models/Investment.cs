using CommunityToolkit.Mvvm.ComponentModel;
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
        private int _id = -1;
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private string _symbol = "";
        [ObservableProperty]
        private float _shares = 1;
        [ObservableProperty]
        private float _value = 0;
        [ObservableProperty]
        private float _total = 0;
        [ObservableProperty]
        private InvestmentType _type = InvestmentType.other;
        [ObservableProperty]
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
}
