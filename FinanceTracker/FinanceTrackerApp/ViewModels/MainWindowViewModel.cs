using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace FinanceTrackerApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private Data _accounts = new();
        [ObservableProperty]
        private AccountsViewModel _accountsViewModel;
        [ObservableProperty]
        private InvestingAccountsViewModel _investingAccountsViewModel;
        [ObservableProperty]
        private BudgetViewModel _budgetViewModel;

        public MainWindowViewModel()
        {
            InvestingAccountsViewModel = new(Accounts);
            AccountsViewModel = new(Accounts);
            BudgetViewModel = new(Accounts);
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void Closing()
        {
            AccountsViewModel.Closing();
            InvestingAccountsViewModel.Closing();
            BudgetViewModel.Closing();
        }
    }
}
