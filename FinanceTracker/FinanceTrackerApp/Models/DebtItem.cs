using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class DebtItem : ObservableObject
    {
        [ObservableProperty]
        private int _id = -1;
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private string _accountId = "";
        [ObservableProperty]
        private float _total = 0;
        [ObservableProperty]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    }
}
