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
        private static readonly string s_budgetFileName = Path.Combine(s_appDataFolder, "Budget.csv");
        [ObservableProperty]
        private Array _budgetItemTypes = Enum.GetValues(typeof(BudgetItemType));
        [ObservableProperty]
        private Array _frequencyTypes = Enum.GetValues(typeof(FrequencyType));
        [ObservableProperty]
        private Accounts _accounts;
        [ObservableProperty]
        private ObservableCollection<BudgetItem> _budgetItems = new();
        [ObservableProperty]
        private int _budgetTotal = 0;
        [ObservableProperty]
        private BudgetItem? _selectedBudgetItem;

        public BudgetViewModel(Accounts accounts)
        {
            Accounts = accounts;
            BudgetItems = GetData<BudgetItem, BudgetItemMap>(s_budgetFileName);
            Update();
        }

        public override void Closing()
        {
            SetData<BudgetItem, BudgetItemMap>(s_budgetFileName, BudgetItems);
        }

        [RelayCommand]
        private void Add()
        {
            BudgetItems.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBudgetItem != null)
            {
                BudgetItems.Remove(SelectedBudgetItem);
            }
        }

        [RelayCommand]
        private void Update()
        {
            foreach(BudgetItem item in BudgetItems)
            {
                if (!string.IsNullOrWhiteSpace(item.AccountName) && item.AccountName != "None")
                {
                    item.Account = Accounts.AccountList.Where(account => account.Name == item.AccountName).First();
                    item.AccountId = item.Account.Id;
                }
                else if (item.Account != null && item.AccountName == "None")
                {
                    item.Account = null;
                    item.AccountId = "";
                }
                else if (!string.IsNullOrWhiteSpace(item.AccountId))
                {
                    item.Account = Accounts.AccountList.Where(account => account.Id == item.AccountId).First();
                    item.AccountName = item.Account.Name;
                }
            }
        }
    }
}
