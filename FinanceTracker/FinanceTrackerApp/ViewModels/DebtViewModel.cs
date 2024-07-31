using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class DebtViewModel : ObservableObject
    {
        [ObservableProperty]
        private Data _data;

        public DebtViewModel(Data data)
        {
            Data = data;
        }
    }
}
