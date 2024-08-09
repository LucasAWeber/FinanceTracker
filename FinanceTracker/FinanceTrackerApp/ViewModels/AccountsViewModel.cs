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
        private Controller _controller;
        [ObservableProperty]
        private float _accountsTotal = 0;
        [ObservableProperty]
        private Account? _selectedAccount;

        public AccountsViewModel(Controller controller)
        {
            Controller = controller;
            Task.Run(() =>
            {
                Controller.GetAccounts();
                Update();
            });
            
        }

        public override void Closing()
        {
            Task.Run(() =>
            {
                Controller.SetAccounts();
            });
        }

        [RelayCommand]
        private void Add()
        {
            Controller.AccountList.Add(new(Controller.Date));
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAccount != null)
            {
                Controller.DeleteAccount(SelectedAccount);
            }
        }

        [RelayCommand]
        private void Update()
        {
            AccountsTotal = 0;
            foreach (Account account in Controller.AccountList)
            {
                AccountsTotal += account.Total;
            }
        }
    }
}
