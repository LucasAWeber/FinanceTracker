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
        [ObservableProperty]
        private Controller _data;
        [ObservableProperty]
        private float _accountsTotal = 0;
        [ObservableProperty]
        private Account? _selectedAccount;

        public AccountsViewModel(Controller data)
        {
            Data = data;
            Data.GetAccounts();
            Update();
        }

        public override void Closing()
        {
            Data.SetAccounts();
        }

        [RelayCommand]
        private void Add()
        {
            Data.AccountList.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAccount != null)
            {
                Data.DeleteAccount(SelectedAccount);
            }
        }

        [RelayCommand]
        private void Update()
        {
            AccountsTotal = 0;
            foreach (Account account in Data.AccountList)
            {
                AccountsTotal += account.Total;
            }
        }
    }
}
