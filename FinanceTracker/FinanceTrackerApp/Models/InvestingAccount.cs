using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class InvestingAccount : ObservableObject
    {
        [ObservableProperty]
        [Index(0)]
        private string _id = Guid.NewGuid().ToString("N");
        [ObservableProperty]
        [Index(1)]
        private string _name = "";
        [ObservableProperty]
        [Index(2)]
        private double _total = 0;
        [ObservableProperty]
        [Index(3)]
        private InvestingAccountType _type = InvestingAccountType.nonregistered;
        [ObservableProperty]
        [Index(4)]
        private ObservableCollection<Investment> _investments = new();

        public async Task UpdateInvestmentAccount()
        {
            Total = 0;
            try
            {
                foreach (Investment investment in Investments)
                {
                    await investment.UpdateInvestment();
                    Total += investment.Total;
                }
            } catch (InvalidOperationException e)
            {
                Debug.WriteLine($"{e.Message}");
            }
            
        }
    }

    public sealed class InvestingAccountMap : ClassMap<InvestingAccount>
    {
        public InvestingAccountMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Total).Index(2);
            Map(m => m.Type).Index(3);
            Map(m => m.Investments).Ignore();
        }
    }
}
