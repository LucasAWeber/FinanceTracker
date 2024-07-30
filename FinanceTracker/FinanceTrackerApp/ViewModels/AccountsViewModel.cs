﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class AccountsViewModel : TabViewModelBase
    {
        private static readonly string s_accountsFileName = Path.Combine(s_appDataFolder, "Accounts.csv");
        [ObservableProperty]
        private ObservableCollection<Account> _accounts = new();
        [ObservableProperty]
        private int _accountsTotal = 0;
        [ObservableProperty]
        private Account? _selectedAccount;

        public AccountsViewModel()
        {
            Accounts = GetData<Account, AccountMap>(s_accountsFileName);
            CalcAccountTotal();
        }

        public override void Closing()
        {
            SetData<Account, AccountMap>(s_accountsFileName, Accounts);
        }

        [RelayCommand]
        private void Add()
        {
            Accounts.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAccount != null)
            {
                Accounts.Remove(SelectedAccount);
            }
        }

        [RelayCommand]
        private void CalcAccountTotal()
        {
            AccountsTotal = 0;
            foreach (Account account in Accounts)
            {
                AccountsTotal += account.Total;
            }
        }
    }
}
