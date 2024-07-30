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
        private ObservableCollection<BudgetItem> _budgetItems = new();
        [ObservableProperty]
        private int _budgetTotal = 0;
        [ObservableProperty]
        private BudgetItem? _selectedBudgetItem;

        public BudgetViewModel()
        {
            BudgetItems = GetData<BudgetItem, BudgetItemMap>(s_budgetFileName);
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
    }
}
