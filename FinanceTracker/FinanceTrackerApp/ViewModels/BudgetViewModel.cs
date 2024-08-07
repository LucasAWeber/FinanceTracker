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
    public partial class BudgetViewModel : TabViewModelBase
    {
        [ObservableProperty]
        private Array _budgetItemTypes = Enum.GetValues(typeof(BudgetItemType));
        [ObservableProperty]
        private Array _frequencyTypes = Enum.GetValues(typeof(FrequencyType));
        [ObservableProperty]
        private Data _data;
        
        [ObservableProperty]
        private float _budgetDailyTotal = 0;
        [ObservableProperty]
        private float _budgetWeeklyTotal = 0;
        [ObservableProperty]
        private float _budgetMonthlyTotal = 0;
        [ObservableProperty]
        private float _budgetYearlyTotal = 0;
        [ObservableProperty]
        private BudgetItem? _selectedBudgetItem;

        public BudgetViewModel(Data data)
        {
            Data = data;

            Update();
        }

        public override void Closing()
        {

        }

        [RelayCommand]
        private void Add()
        {
            Data.BudgetItems.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBudgetItem != null)
            {
                Data.BudgetItems.Remove(SelectedBudgetItem);
            }
        }

        [RelayCommand]
        private void Update()
        {
            float dailyTotal = 0;
            float weeklyTotal = 0;
            float monthlyTotal = 0;
            float yearlyTotal = 0;

            /*foreach(BudgetItem item in Data.BudgetItems)
            {
                // Handles the account linking logic
                if (!string.IsNullOrWhiteSpace(item.AccountName) && item.AccountName != "None")
                {
                    //item.Account = Data.AccountList.Where(account => account.Name == item.AccountName).First();
                    //item.AccountId = item.Account.Id;
                }
                else if (item.Account != null && item.AccountName == "None")
                {
                    //item.Account = null;
                    //item.AccountId = -1;
                }
                else if (!string.IsNullOrWhiteSpace(item.AccountId))
                {
                    //item.Account = Data.AccountList.Where(account => account.Id == item.AccountId).First();
                    //item.AccountName = item.Account.Name;
                }

                // Handles the total budget values
                switch(item.Frequency)
                {
                    case FrequencyType.Daily:
                        dailyTotal += item.Total;
                        weeklyTotal += item.Total * 7;
                        monthlyTotal += item.Total * 30;
                        yearlyTotal += item.Total * 365;
                        break;
                    case FrequencyType.Weekly:
                        dailyTotal += item.Total / 7;
                        weeklyTotal += item.Total;
                        monthlyTotal += item.Total * 4;
                        yearlyTotal += item.Total * 52;
                        break;
                    case FrequencyType.Monthly:
                        dailyTotal += item.Total / 30;
                        weeklyTotal += item.Total / 4;
                        monthlyTotal += item.Total;
                        yearlyTotal += item.Total * 12;
                        break;
                    case FrequencyType.Yearly:
                        dailyTotal += item.Total / 365;
                        weeklyTotal += item.Total / 52;
                        monthlyTotal += item.Total / 12;
                        yearlyTotal += item.Total;
                        break;
                }
            }*/
            BudgetDailyTotal = dailyTotal;
            BudgetWeeklyTotal = weeklyTotal;
            BudgetMonthlyTotal = monthlyTotal;
            BudgetYearlyTotal = yearlyTotal;
        }
    }
}
