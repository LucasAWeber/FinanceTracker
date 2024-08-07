using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinanceTrackerApp.ViewModels
{
    public partial class InvestingAccountsViewModel : TabViewModelBase
    {
        [ObservableProperty]
        private Array _investingAccountTypes = Enum.GetValues(typeof(InvestingAccountType));
        [ObservableProperty]
        private Array _investmentTypes = Enum.GetValues(typeof(InvestmentType));
        [ObservableProperty]
        private Array _stockExchanges = Enum.GetValues(typeof(StockExchange));
        [ObservableProperty]
        private Controller _data;
        [ObservableProperty]
        private float _investingAccountsTotal = 0;
        [ObservableProperty]
        private InvestingAccount? _selectedInvestingAccount;
        [ObservableProperty]
        private Investment? _selectedInvestment;

        public InvestingAccountsViewModel(Controller data)
        {
            Data = data;
            Data.GetInvestingAccounts();
            _ = Update();
        }   

        public override void Closing()
        {
            Data.SetInvestingAccounts();
        }

        [RelayCommand]
        private void AddAccount()
        {
            Data.InvestingAccountList.Add(new());
        }

        [RelayCommand]
        private void AddInvestment()
        {
            SelectedInvestingAccount?.Investments.Add(new());
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            if (SelectedInvestingAccount != null)
            {
                Data.DeleteInvestingAccount(SelectedInvestingAccount);
            }
        }

        [RelayCommand]
        private void DeleteInvestment()
        {
            if (SelectedInvestment != null)
            {
                SelectedInvestingAccount?.Investments.Remove(SelectedInvestment);
            }
        }

        [RelayCommand]
        private async Task Update()
        {
            float total = 0;
            foreach (InvestingAccount account in Data.InvestingAccountList)
            {
                await account.UpdateInvestmentAccount();
                total += account.Total;
            }
            InvestingAccountsTotal = total;
        }
    }
}
