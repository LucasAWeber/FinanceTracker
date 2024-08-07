using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class AccountsViewModel : TabViewModelBase
    {
        private static readonly string s_accountsFileName = Path.Combine(s_appDataFolder, "Accounts.csv");
        [ObservableProperty]
        private Data _data;
        [ObservableProperty]
        private float _accountsTotal = 0;
        [ObservableProperty]
        private Account? _selectedAccount;

        public AccountsViewModel(Data data)
        {
            Data = data;
            //Data.AccountIds = GetData<string>(s_accountsFileName);
            //Data.AccountList[Today] = GetData<Account, AccountMap>(s_accountsFileName);
            //Data.AccountList = GetData<Account, AccountMap>(s_accountsFileName);
            Data.GetAccounts(Data.Date);
            Update();
        }

        public override void Closing()
        {
            Data.SetAccounts();
            //SetData<string>(s_accountsFileName, Data.AccountIds);
            //foreach (string id in Data.AccountIds)
            //{
            //SetData<Account, AccountMap>(Path.Combine(s_appDataFolder, id + ".csv"), Data.AccountList[id]);
            //}
            //SetData<Account, AccountMap>(s_accountsFileName, Data.AccountList);
        }

        [RelayCommand]
        private void Add()
        {
            Data.AccountList.Add(new());
        }

        [RelayCommand]
        private void Delete()
        {
            //if (SelectedAccount != null)
            //{
                //Data.AccountList.Remove(SelectedAccount);
            //}
        }

        [RelayCommand]
        private void Update()
        {
            //AccountsTotal = 0;
            //foreach (Account account in Data.AccountList)
            //{
                //AccountsTotal += account.Total;
            //}
        }
    }
}
