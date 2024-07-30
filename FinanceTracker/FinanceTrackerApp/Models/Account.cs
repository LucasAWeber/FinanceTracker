using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
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
        [Index(0)]
        private string _id = Guid.NewGuid().ToString("N");
        [ObservableProperty]
        [Index(1)]
        private string _name = "";
        [ObservableProperty]
        [Index(2)]
        private int _total = 0;
        [ObservableProperty]
        [Index(3)]
        private float _interest = 0;
        [ObservableProperty]
        [Index(4)]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    }

    public sealed class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Total).Index(2);
            Map(m => m.Interest).Index(3);
            Map(m => m.Date).Index(4);
        }
    }
}
