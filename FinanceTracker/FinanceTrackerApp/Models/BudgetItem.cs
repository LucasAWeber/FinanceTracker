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
        private float _total = 0;
        [ObservableProperty]
        [Index(2)]
        private string _accountId = "";
        [ObservableProperty]
        private string _accountName = "None";
        [ObservableProperty]
        private Account? _account;
        [ObservableProperty]
        [Index(3)]
        private string _investingAccountId = "";
        [ObservableProperty]
        private string _investingAccountName = "None";
        [ObservableProperty]
        private Account? _investingAccount;
        [ObservableProperty]
        [Index(4)]
        private FrequencyType _frequency = FrequencyType.Weekly;
        [ObservableProperty]
        [Index(5)]
        private BudgetItemType _type = BudgetItemType.Other;
        [ObservableProperty]
        [Index(6)]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    }

    public sealed class BudgetItemMap : ClassMap<BudgetItem>
    {
        public BudgetItemMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Total).Index(1);
            Map(m => m.AccountId).Index(2);
            Map(m => m.InvestingAccountId).Index(3);
            Map(m => m.Frequency).Index(4);
            Map(m => m.Type).Index(5);
            Map(m => m.Date).Index(6);
            Map(m => m.Account).Ignore();
            Map(m => m.AccountName).Ignore();
            Map(m => m.InvestingAccount).Ignore();
            Map(m => m.InvestingAccountName).Ignore();
        }
    }
}
