using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class Controller : ObservableObject
    {
        [ObservableProperty]
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
        private static DateOnly Today
        {
            get
            {
                return DateOnly.FromDateTime(DateTime.Now);
            }
        }
        [ObservableProperty]
        private ObservableCollection<Account> _accountList = new();
        [ObservableProperty]
        private ObservableCollection<InvestingAccount> _investingAccountList = new();
        [ObservableProperty]
        private ObservableCollection<BudgetItem> _budgetList = new();
        [ObservableProperty]
        private ObservableCollection<DebtItem> _debtList = new();

        [ObservableProperty]
        private float _accountsTotal = 0;
        [ObservableProperty]
        private float _investingAccountsTotal = 0;
        [ObservableProperty]
        private float _DebtTotal;

        public Controller()
        {
            Database.CreateDatabase();
        }

        public void IncrementDate()
        {
            if (Date < Today)
            {
                SetAccounts();
                SetInvestingAccounts();
                Date = Date.AddDays(1);
                GetAccounts();
                GetInvestingAccounts();
            }
        }

        public void DecrementDate()
        {
            SetAccounts();
            SetInvestingAccounts();
            Date = Date.AddDays(-1);
            GetAccounts();
            GetInvestingAccounts();
        }

        public void DeleteAccount(Account account)
        {
            AccountList.Remove(account);
            Task.Run(() => Database.DeleteAccount(account));
        }

        public void GetAccounts()
        {
            AccountList = Database.GetAccounts(Date);
        }

        public void UpdateAccounts()
        {
            float total = 0;
            foreach (Account account in AccountList)
            {
                total += account.Total;
            }
            AccountsTotal = total;
        }

        public void SetAccounts()
        {
            Database.SetAccounts(AccountList);
        }

        public void DeleteInvestingAccount(InvestingAccount account)
        {
            InvestingAccountList.Remove(account);
            Task.Run(() => Database.DeleteInvestingAccount(account));
        }

        public void DeleteInvestment(InvestingAccount account, Investment investment)
        {
            account.Investments.Remove(investment);
            Task.Run(() => Database.DeleteInvestment(investment));
        }

        public void GetInvestingAccounts()
        {
            InvestingAccountList = Database.GetInvestingAccounts(Date);
        }

        public async Task UpdateInvestingAccounts()
        {
            float total = 0;
            foreach (InvestingAccount account in InvestingAccountList)
            {
                await account.UpdateInvestmentAccount();
                total += account.Total;
            }
            InvestingAccountsTotal = total;
        }

        public void SetInvestingAccounts()
        {
            Database.SetInvestingAccounts(InvestingAccountList);
        }
    }
}
