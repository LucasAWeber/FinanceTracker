﻿using System;
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
        private Controller _controller = new();
        [ObservableProperty]
        private AccountsViewModel _accountsViewModel;
        [ObservableProperty]
        private InvestingAccountsViewModel _investingAccountsViewModel;
        [ObservableProperty]
        private BudgetViewModel _budgetViewModel;
        [ObservableProperty]
        private DebtViewModel _debtViewModel;

        public MainWindowViewModel()
        {
            InvestingAccountsViewModel = new(Controller);
            AccountsViewModel = new(Controller);
            BudgetViewModel = new(Controller);
            DebtViewModel = new(Controller);
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
            DebtViewModel.Closing();
        }

        private async Task Update()
        {
            await AccountsViewModel.Update();
            await InvestingAccountsViewModel.Update();
            await BudgetViewModel.Update();
            await DebtViewModel.Update();
        }

        [RelayCommand]
        private async Task DecrementDate()
        {
            Controller.DecrementDate();
            await Update();
        }

        [RelayCommand]
        private async Task IncrementDate()
        {
            Controller.IncrementDate();
            await Update();
        }
    }
}
