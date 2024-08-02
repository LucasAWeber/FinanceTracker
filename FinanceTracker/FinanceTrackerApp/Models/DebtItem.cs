using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration.Attributes;
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
        [Index(0)]
        private string _id = Guid.NewGuid().ToString("N");
        [ObservableProperty]
        [Index(1)]
        private string _name = "";
        [ObservableProperty]
        [Index(2)]
        private string _accountId = "";
        [ObservableProperty]
        [Index(3)]
        private float _total = 0;
        [ObservableProperty]
        [Index(4)]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    }
}
