using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.Models
{
    public partial class Accounts : ObservableObject
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
                foreach(Account account in _accountList)
                {
                    AccountNameList.Add(account.Name);
                }
            }
        }
    }
}
