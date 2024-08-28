using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class DebtViewModel : TabViewModelBase
    {
        [ObservableProperty]
        private Controller _data;
        [ObservableProperty]
        private float _DebtTotal;
        [ObservableProperty]
        private DebtItem? _selectedDebtItem;

        public DebtViewModel(Controller data)
        {
            Data = data;
            Update();
        }

        public override void Closing()
        {

        }

        [RelayCommand]
        public override async Task Update()
        {
            float total = 0;
            foreach (DebtItem item in Data.DebtList)
            {
                total += item.Total;
            }
            DebtTotal = total;
        }

        [RelayCommand]
        private void Add()
        {
            Data.DebtList.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedDebtItem != null)
            {
                Data.DebtList.Remove(SelectedDebtItem);
            }
        }
    }
}
