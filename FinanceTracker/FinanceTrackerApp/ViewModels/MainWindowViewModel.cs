using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;

namespace FinanceTrackerApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private static readonly string s_appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker");
        private static readonly string s_accountsFileName = Path.Combine(s_appDataFolder, "Accounts.csv");
        private static readonly string s_investingAccountsFileName = Path.Combine(s_appDataFolder, "InvestingAccounts.csv");
        private static readonly CsvConfiguration s_csvConfig = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
        };

        [ObservableProperty]
        private ObservableCollection<Account> _accounts = new();
        [ObservableProperty]
        private int _accountsTotal = 0;

        [ObservableProperty]
        private ObservableCollection<InvestingAccount> _investingAccounts = new();

        public MainWindowViewModel()
        {
            ImportData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static ObservableCollection<T> GetData<T>(string filename)
        {
            FileInfo myFile = new(filename);
            myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamReader reader = new(filename);
            using CsvReader csvRead = new(reader, s_csvConfig);
            IEnumerable<T> records = csvRead.GetRecords<T>();
            myFile.Attributes |= FileAttributes.Hidden;
            return new ObservableCollection<T>(records);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        private static void SetData<T>(string filename, ObservableCollection<T> data)
        {
            FileInfo myFile = new(filename);
            myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamWriter writer = new(filename);
            using CsvWriter csvWrite = new(writer, s_csvConfig);
            csvWrite.WriteRecords(data);
            myFile.Attributes |= FileAttributes.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void SaveData()
        {
            SetData(s_accountsFileName, Accounts);
            SetData(s_investingAccountsFileName, InvestingAccounts);
        }

        private void ImportData()
        {
            if (!Directory.Exists(s_appDataFolder))
            {
                Directory.CreateDirectory(s_appDataFolder);
            }

            if (!File.Exists(s_accountsFileName))
            {
                File.Create(s_accountsFileName).Close();
                File.SetAttributes(s_accountsFileName, File.GetAttributes(s_accountsFileName) | FileAttributes.Hidden);
                File.Encrypt(s_accountsFileName);
            }
            Accounts = GetData<Account>(s_accountsFileName);

            if (!File.Exists(s_investingAccountsFileName))
            {
                File.Create(s_investingAccountsFileName).Close();
                File.SetAttributes(s_investingAccountsFileName, File.GetAttributes(s_investingAccountsFileName) | FileAttributes.Hidden);
                File.Encrypt(s_investingAccountsFileName);
            }
            InvestingAccounts = GetData<InvestingAccount>(s_investingAccountsFileName);
        }
    }
}
