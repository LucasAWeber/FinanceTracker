using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class Account : ObservableObject
    {
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private int _total = 0;
        [ObservableProperty]
        private float _interest = 0;
    }
}
