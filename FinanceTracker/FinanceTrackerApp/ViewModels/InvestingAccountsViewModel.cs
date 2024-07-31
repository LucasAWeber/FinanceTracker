﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinanceTrackerApp.ViewModels
{
    public partial class InvestingAccountsViewModel : TabViewModelBase
    {
        private static readonly string s_investingAccountsFileName = Path.Combine(s_appDataFolder, "InvestingAccounts.csv");
        [ObservableProperty]
        private Array _investingAccountTypes = Enum.GetValues(typeof(InvestingAccountType));
        [ObservableProperty]
        private Array _investmentTypes = Enum.GetValues(typeof(InvestmentType));
        [ObservableProperty]
        private Array _stockExchanges = Enum.GetValues(typeof(StockExchange));
        [ObservableProperty]
        private Accounts _accounts;
        [ObservableProperty]
        private float _investingAccountsTotal = 0;
        [ObservableProperty]
        private InvestingAccount? _selectedInvestingAccount;
        [ObservableProperty]
        private Investment? _selectedInvestment;

        public InvestingAccountsViewModel(Accounts accounts)
        {
            Accounts = accounts;
            Accounts.InvestingAccountList = GetData<InvestingAccount, InvestingAccountMap>(s_investingAccountsFileName);
            foreach(InvestingAccount account in Accounts.InvestingAccountList)
            {
                account.Investments = GetData<Investment, InvestmentMap>(Path.Combine(s_appDataFolder,account.Id + ".csv"));
            }
            _ = Update();
        }   

        public override void Closing()
        {
            foreach (InvestingAccount account in Accounts.InvestingAccountList)
            {
                SetData<Investment, InvestmentMap>(Path.Combine(s_appDataFolder, account.Id + ".csv"), account.Investments);
            }
            SetData<InvestingAccount, InvestingAccountMap>(s_investingAccountsFileName, Accounts.InvestingAccountList);
        }

        [RelayCommand]
        private void AddAccount()
        {
            Accounts.InvestingAccountList.Add(new());
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
                Accounts.InvestingAccountList.Remove(SelectedInvestingAccount);
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
            foreach (InvestingAccount account in Accounts.InvestingAccountList)
            {
                await account.UpdateInvestmentAccount();
                total += account.Total;
            }
            InvestingAccountsTotal = total;
        }
    }
}
