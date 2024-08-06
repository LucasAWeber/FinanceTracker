using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data.Entity;
using System.Collections.ObjectModel;
using FinanceTrackerApp.Models;
using System.Diagnostics;
using Flurl.Util;
using System.Security.Principal;

namespace FinanceTrackerApp.Helpers
{
    public class Database
    {
        protected static readonly string s_appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker");
        private static readonly string _filename = Path.Combine(s_appDataFolder,"FinanceTracker.sqlite");
        private static readonly string _connectionString = $"Data Source={_filename};";
        private SQLiteConnection _connection;

        public Database()
        {
            if (!Directory.Exists(s_appDataFolder))
            {
                Directory.CreateDirectory(s_appDataFolder);
            }
            if (!File.Exists(_filename))
            {
                CreateDatabase();
            } else
            {
                _connection = new SQLiteConnection(_connectionString);
            }
        }

        private void CreateDatabase()
        {
            string commandString;
            SQLiteCommand command;
            SQLiteConnection.CreateFile(_filename);
            _connection = new SQLiteConnection(_connectionString);
            _connection.Open();
            commandString = $"CREATE TABLE account (account_id INT PRIMARY KEY, account_name VARCHAR(100), account_total REAL, account_interest REAL, account_date VARCHAR(10));";
            command = new(commandString, _connection);
            command.ExecuteNonQuery();
            _connection.Close();
        }

        public ObservableCollection<Account> GetAccounts(DateOnly date)
        {
            //SELECT account_id, account_name, account_total, account_interest, MAX(account_date) FROM account GROUP BY account_id
            ObservableCollection<Account> accounts = new();
            _connection.Open();
            string commandString = $"SELECT account_id, account_name, account_total, account_interest, account_date FROM account WHERE account_date='{date.ToInvariantString()}'  GROUP BY account_id";
            SQLiteCommand command = new(commandString, _connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Debug.WriteLine(reader["account_date"].ToString());
                accounts.Add(new()
                {
                    Id = int.Parse(reader["account_id"].ToString()),
                    Name = reader["account_name"].ToString(),
                    Total = float.Parse(reader["account_id"].ToString()),
                    Interest = float.Parse(reader["account_interest"].ToString()),
                    Date = DateOnly.ParseExact(reader["account_date"].ToString(), "MM/dd/yyyy")
                });
            }
            _connection.Close();
            return accounts;
        }

        public void SetAccounts(ObservableCollection<Account> accounts)
        {
            string commandString;
            SQLiteCommand command;
            SQLiteDataReader reader;
            foreach (Account account in accounts)
            {
                int index = accounts.IndexOf(account);
                _connection.Open();
                commandString = $"SELECT * FROM account WHERE account_id={index} AND account_date='{account.Date.ToInvariantString()}'";
                command = new(commandString, _connection);
                reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    _connection.Close();
                    _connection.Open();
                    commandString = $"INSERT INTO account (account_id, account_name, account_total, account_interest, account_date) VALUES ('{index}', '{account.Name}', {account.Total}, {account.Interest}, '{account.Date.ToInvariantString()}');";
                    command = new(commandString, _connection);
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                else
                {
                    _connection.Close();
                    _connection.Open();
                    commandString = $"UPDATE account SET account_total={account.Total}, account_interest={account.Interest}";
                    command = new(commandString, _connection);
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
            }
        }
    }
}
