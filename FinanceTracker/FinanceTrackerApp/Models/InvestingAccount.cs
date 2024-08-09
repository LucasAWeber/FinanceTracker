using CommunityToolkit.Mvvm.ComponentModel;
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
        private int _id = -1;
        [ObservableProperty]
        private int _infoId = -1;
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private float _total = 0;
        [ObservableProperty]
        private InvestingAccountType _type = InvestingAccountType.nonregistered;
        [ObservableProperty]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
        [ObservableProperty]
        private ObservableCollection<Investment> _investments = new();

        public InvestingAccount(DateOnly? date = null)
        {
            if (date != null)
            {
                Date = (DateOnly)date;
            }
        }

        public async Task UpdateInvestmentAccount()
        {
            float total = 0;
            try
            {
                foreach (Investment investment in Investments)
                {
                    await investment.UpdateInvestment();
                    total += investment.Total;
                }
                Total = total;
            } catch (InvalidOperationException e)
            {
                Debug.WriteLine($"{e.Message}");
            }
        }
    }
}
