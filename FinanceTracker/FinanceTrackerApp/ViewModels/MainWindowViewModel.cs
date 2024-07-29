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
        private AccountsViewModel _accountsViewModel = new();
        [ObservableProperty]
        private InvestingAccountsViewModel _investingAccountsViewModel = new();

        public MainWindowViewModel()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void Closing()
        {
            AccountsViewModel.Closing();
            InvestingAccountsViewModel.Closing();
        }
    }
}
