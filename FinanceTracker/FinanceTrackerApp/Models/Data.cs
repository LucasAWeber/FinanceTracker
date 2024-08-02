using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class Data : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> _accountNameList = new();
        private ObservableCollection<Account> _accountList = new();
        public ObservableCollection<Account> AccountList
        {
            get => _accountList;
            set
            {
                SetProperty(ref _accountList, value);
                AccountNameList.Clear();
                AccountNameList.Add("None");
                foreach(Account account in _accountList)
                {
                    AccountNameList.Add(account.Name);
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<string> _investingAccountNameList = new();
        private ObservableCollection<InvestingAccount> _investingAccountList = new();
        public ObservableCollection<InvestingAccount> InvestingAccountList
        {
            get => _investingAccountList;
            set
            {
                SetProperty(ref _investingAccountList, value);
                InvestingAccountNameList.Clear();
                InvestingAccountNameList.Add("None");
                foreach (InvestingAccount investingAccount in _investingAccountList)
                {
                    InvestingAccountNameList.Add(investingAccount.Name);
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<BudgetItem> _budgetItems = new();

        [ObservableProperty]
        private ObservableCollection<BudgetItem> _debtList = new();
    }
}
