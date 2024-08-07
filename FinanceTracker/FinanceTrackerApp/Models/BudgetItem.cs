using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class BudgetItem : ObservableObject
    {
        [ObservableProperty]
        private int _id = -1;
        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private float _total = 0;
        [ObservableProperty]
        private string _accountName = "None";
        [ObservableProperty]
        private Account? _account;
        [ObservableProperty]
        private string _investingAccountName = "None";
        [ObservableProperty]
        private Account? _investingAccount;
        [ObservableProperty]
        private FrequencyType _frequency = FrequencyType.Weekly;
        [ObservableProperty]
        private BudgetItemType _type = BudgetItemType.Other;
        [ObservableProperty]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    }
}
