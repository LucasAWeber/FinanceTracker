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
    public partial class AccountsViewModel : TabViewModelBase
    {
        private static readonly string s_accountsFileName = Path.Combine(s_appDataFolder, "Accounts.csv");
        [ObservableProperty]
        private Accounts _accounts;
        [ObservableProperty]
        private float _accountsTotal = 0;
        [ObservableProperty]
        private Account? _selectedAccount;

        public AccountsViewModel(Accounts accounts)
        {
            Accounts = accounts;
            Accounts.AccountList = GetData<Account, AccountMap>(s_accountsFileName);
            Update();
        }

        public override void Closing()
        {
            SetData<Account, AccountMap>(s_accountsFileName, Accounts.AccountList);
        }

        [RelayCommand]
        private void Add()
        {
            Accounts.AccountList.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAccount != null)
            {
                Accounts.AccountList.Remove(SelectedAccount);
            }
        }

        [RelayCommand]
        private void Update()
        {
            AccountsTotal = 0;
            foreach (Account account in Accounts.AccountList)
            {
                AccountsTotal += account.Total;
            }
        }
    }
}
