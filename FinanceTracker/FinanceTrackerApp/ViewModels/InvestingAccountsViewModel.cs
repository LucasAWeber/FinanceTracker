using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class InvestingAccountsViewModel : AccountViewModelBase
    {
        private static readonly string s_investingAccountsFileName = Path.Combine(s_appDataFolder, "InvestingAccounts.csv");
        [ObservableProperty]
        private Array _investingAccountTypes = Enum.GetValues(typeof(InvestingAccountType));
        [ObservableProperty]
        private ObservableCollection<InvestingAccount> _investingAccounts = new();
        [ObservableProperty]
        private int _investingAccountsTotal = 0;

        public InvestingAccountsViewModel()
        {
            InvestingAccounts = GetData<InvestingAccount>(s_investingAccountsFileName);
            CalcInvestingAccountTotal();
        }

        public void Closing()
        {
            SetData(s_investingAccountsFileName, InvestingAccounts);
        }

        [RelayCommand]
        private void CalcInvestingAccountTotal()
        {
            InvestingAccountsTotal = 0;
            foreach (InvestingAccount account in InvestingAccounts)
            {
                InvestingAccountsTotal += account.Total;
            }
        }
    }
}
