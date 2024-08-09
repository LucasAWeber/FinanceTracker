using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class Controller : ObservableObject
    {
        [ObservableProperty]
        private Database _database = new();
        [ObservableProperty]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
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
        private ObservableCollection<string> _investingAccountIds = new();
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


        public void DeleteAccount(Account account)
        {
            AccountList.Remove(account);
            Database.DeleteAccount(account);
        }

        public void GetAccounts()
        {
            AccountList = Database.GetAccounts(Date);
        }

        public void SetAccounts()
        {
            Database.SetAccounts(AccountList);
        }

        public void DeleteInvestingAccount(InvestingAccount account)
        {
            InvestingAccountList.Remove(account);
            Database.DeleteInvestingAccount(account);
        }

        public void DeleteInvestment(InvestingAccount account, Investment investment)
        {
            account.Investments.Remove(investment);
            Database.DeleteInvestment(investment);
        }

        public void GetInvestingAccounts()
        {
            InvestingAccountList = Database.GetInvestingAccounts(Date);
        }

        public void SetInvestingAccounts()
        {
            Database.SetInvestingAccounts(InvestingAccountList);
        }
    }
}
