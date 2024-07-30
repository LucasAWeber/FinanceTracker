using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace FinanceTrackerApp.Models
{
    public partial class BudgetItem : ObservableObject
    {
        [ObservableProperty]
        [Index(0)]
        private string _name = "";
        [ObservableProperty]
        [Index(1)]
        private double _total = 0;
        [ObservableProperty]
        [Index(2)]
        private FrequencyType _frequency = FrequencyType.Yearly;
        [ObservableProperty]
        [Index(3)]
        private BudgetItemType _type = BudgetItemType.Other;
    }

    public sealed class BudgetItemMap : ClassMap<BudgetItem>
    {
        public BudgetItemMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Total).Index(1);
            Map(m => m.Frequency).Index(2);
            Map(m => m.Type).Index(3);
        }
    }
}
