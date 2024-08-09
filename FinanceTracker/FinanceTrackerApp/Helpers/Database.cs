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
using System.Data.Common;

namespace FinanceTrackerApp.Helpers
{
    public class Database
    {
        private static readonly string s_appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker");
        private static readonly string _filename = Path.Combine(s_appDataFolder,"FinanceTracker.sqlite");
        private static readonly string _connectionString = $"Data Source={_filename};";
        private SQLiteConnection _connection;

        public Database()
        {
            if (!Directory.Exists(s_appDataFolder))
            {
                Directory.CreateDirectory(s_appDataFolder);
            }
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            string commandString;
            // create database and set up connection
            if (!File.Exists(_filename))
            {
                SQLiteConnection.CreateFile(_filename);
            }
            _connection = new SQLiteConnection(_connectionString);

            // create account tables
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS account_info (account_info_id INTEGER PRIMARY KEY, account_name VARCHAR(100));";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS account (account_id INTEGER PRIMARY KEY, account_info_id INTEGER NOT NULL, account_total REAL, account_interest REAL, account_date VARCHAR(10), FOREIGN KEY (account_info_id) REFERENCES account_info (account_info_id));";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();

            // create investing account tables
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investing_account_info (investing_account_info_id INTEGER PRIMARY KEY, investing_account_info_name VARCHAR(100), investing_account_info_type INTEGER);";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investing_account (investing_account_id INTEGER PRIMARY KEY, investing_account_info_id INTEGER, investing_account_date VARCHAR(10), FOREIGN KEY (investing_account_info_id) REFERENCES investing_account_info (investing_account_info_id));";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();

            // creates investment table
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investment (investment_id INTEGER PRIMARY KEY, investment_shares INTEGER, investment_item_id INTEGER, investing_account_id INTEGER, FOREIGN KEY (investment_item_id) REFERENCES investment_item (investment_item_id), FOREIGN KEY (investing_account_id) REFERENCES investing_account (investing_account_id));";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();

            // create investment item tables
            _connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investment_item (investment_item_id INTEGER PRIMARY KEY, investment_item_name VARCHAR(100), investment_item_symbol VARCHAR(100), investment_item_type INTEGER, investment_item_stock_exchange INTEGER);";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public void DeleteAccount(Account account)
        {
            _connection.Open();
            string commandString = $"DELETE FROM account WHERE account_info_id={account.Id}";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
            _connection.Open();
            commandString = $"DELETE FROM account_info WHERE account_info_id={account.Id}";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public ObservableCollection<Account> GetAccounts(DateOnly date)
        {
            ObservableCollection<Account> accounts = new();
            _connection.Open();
            string commandString = $"SELECT account_info.account_info_id, account_name, account_total, account_interest, account_date FROM account INNER JOIN account_info ON account_info.account_info_id=account.account_info_id WHERE account_date='{date.ToInvariantString()}'";
            using SQLiteCommand command = new(commandString, _connection);
            using SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                accounts.Add(new()
                {
                    Id = int.Parse(reader["account_info_id"].ToString()),
                    Name = reader["account_name"].ToString(),
                    Total = float.Parse(reader["account_total"].ToString()),
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
            bool NewEntry;
            foreach (Account account in accounts)
            {
                long index = account.Id;
                _connection.Open();
                commandString = $"SELECT * FROM account WHERE account_info_id={index} AND account_date='{account.Date.ToInvariantString()}'";
                using SQLiteCommand selectCommand = new(commandString, _connection);
                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    NewEntry = !reader.Read();
                }
                if (NewEntry)
                {
                    _connection.Close();
                    _connection.Open();
                    commandString = $"INSERT INTO account_info (account_name) VALUES ('{account.Name}');";
                    using (SQLiteCommand insertInfoCommand = new(commandString, _connection))
                    {
                        insertInfoCommand.ExecuteNonQuery();
                    }
                    commandString = $"SELECT last_insert_rowid();";
                    using (SQLiteCommand selectIdCommand = new(commandString, _connection))
                    {
                        using SQLiteDataReader reader = selectIdCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            index = (long)reader[0];
                        }
                    }
                    
                    _connection.Close();
                    _connection.Open();
                    commandString = $"INSERT INTO account (account_info_id, account_total, account_interest, account_date) VALUES ('{index}', {account.Total}, {account.Interest}, '{account.Date.ToInvariantString()}');";
                    using SQLiteCommand insertCommand = new(commandString, _connection);
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    _connection.Close();
                    _connection.Open();
                    commandString = $"UPDATE account_info SET account_name='{account.Name}' WHERE account_info_id={index}";
                    using SQLiteCommand updateNameCommand = new(commandString, _connection);
                    updateNameCommand.ExecuteNonQuery();
                    _connection.Close();
                    _connection.Open();
                    commandString = $"UPDATE account SET account_total={account.Total}, account_interest={account.Interest} WHERE account_info_id={index} AND account_date='{account.Date.ToInvariantString()}'";
                    using SQLiteCommand updateCommand = new(commandString, _connection);
                    updateCommand.ExecuteNonQuery();
                }
                _connection.Close();
            }
        }

        public void DeleteInvestingAccount(InvestingAccount account)
        {
            _connection.Open();
            string commandString = $"DELETE FROM investing_account WHERE investing_account_info_id={account.Id}";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
            _connection.Open();
            commandString = $"DELETE FROM investing_account_info WHERE investing_account_info_id={account.Id}";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public ObservableCollection<InvestingAccount> GetInvestingAccounts(DateOnly date)
        {
            ObservableCollection<InvestingAccount> accounts = new();
            _connection.Open();
            string commandString = $"SELECT investing_account_id, investing_account_info_name, investing_account_info_type, investing_account_date FROM investing_account INNER JOIN investing_account_info ON investing_account_info.investing_account_info_id=investing_account.investing_account_info_id WHERE investing_account_date='{date.ToInvariantString()}'";
            using (SQLiteCommand command = new(commandString, _connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Add(new()
                    {
                        Id = int.Parse(reader["investing_account_id"].ToString()),
                        Name = reader["investing_account_info_name"].ToString(),
                        Type = (InvestingAccountType)Enum.Parse(typeof(InvestingAccountType), reader["investing_account_info_type"].ToString()),
                        Date = DateOnly.ParseExact(reader["investing_account_date"].ToString(), "MM/dd/yyyy")
                    });
                }
            }
            
           
            foreach(InvestingAccount account in accounts)
            {
                _connection.Close();
                _connection.Open();
                commandString = $"SELECT investment_id, investment_shares, investment_item_name, investment_item_symbol, investment_item_type, investment_item_stock_exchange FROM investment INNER JOIN investment_item ON investment.investment_item_id=investment_item.investment_item_id WHERE investing_account_id={account.Id}";
                using SQLiteCommand command = new(commandString, _connection);
                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    account.Investments.Add(new()
                    {
                        Id = int.Parse(reader["investment_id"].ToString()),
                        Name = reader["investment_item_name"].ToString(),
                        Symbol = reader["investment_item_symbol"].ToString(),
                        Shares = float.Parse(reader["investment_shares"].ToString()),
                        Type = (InvestmentType)Enum.Parse(typeof(InvestmentType), reader["investment_item_type"].ToString()),
                        StockExchange = (StockExchange)Enum.Parse(typeof(StockExchange), reader["investment_item_stock_exchange"].ToString())
                    });
                }
            }

            _connection.Close();
            return accounts;
        }

        public void SetInvestingAccounts(ObservableCollection<InvestingAccount> accounts)
        {
            string commandString;
            bool NewEntry;
            foreach (InvestingAccount account in accounts)
            {
                int accountInfoId;

                _connection.Open();
                commandString = $"SELECT exists(SELECT 1 FROM investing_account WHERE investing_account_id={account.Id}) AS row_exists;";
                using SQLiteCommand selectCommand = new(commandString, _connection);
                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    NewEntry = !reader.Read() || int.Parse(reader[0].ToString()) == 0;
                }
                if (NewEntry)
                {
                    _connection.Close();
                    _connection.Open();
                    commandString = $"INSERT INTO investing_account_info (investing_account_info_name, investing_account_info_type) VALUES ('{account.Name}', {(int)account.Type});";
                    using (SQLiteCommand insertInfoCommand = new(commandString, _connection))
                    {
                        insertInfoCommand.ExecuteNonQuery();
                    }

                    string getRowIdString = $"SELECT last_insert_rowid();";
                    using (SQLiteCommand selectIdCommand = new(getRowIdString, _connection))
                    {
                        using SQLiteDataReader reader = selectIdCommand.ExecuteReader();
                        accountInfoId = int.Parse(reader[0].ToString());
                    }
                    

                    _connection.Close();
                    _connection.Open();
                    commandString = $"INSERT INTO investing_account (investing_account_info_id, investing_account_date) VALUES ('{accountInfoId}', '{account.Date.ToInvariantString()}');";
                    using (SQLiteCommand insertCommand = new(commandString, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }

                    int accountId;
                    getRowIdString = $"SELECT last_insert_rowid();";
                    using (SQLiteCommand selectIdCommand = new(getRowIdString, _connection))
                    {
                        using SQLiteDataReader reader = selectIdCommand.ExecuteReader();
                        accountId = int.Parse(reader[0].ToString());
                    }
                    
                    foreach (Investment investment in account.Investments)
                    {
                        int investmentItemId;
                        _connection.Close();
                        _connection.Open();
                        commandString = $"INSERT INTO investment_item (investment_item_name, investment_item_symbol, investment_item_type, investment_item_stock_exchange) VALUES ('{investment.Name}', '{investment.Symbol}', {(int)investment.Type}, {(int)investment.StockExchange});";
                        using (SQLiteCommand insertCommand = new(commandString, _connection)) {
                            insertCommand.ExecuteNonQuery();
                        }

                        getRowIdString = $"SELECT last_insert_rowid();";
                        using (SQLiteCommand selectIdCommand = new(getRowIdString, _connection))
                        {
                            using SQLiteDataReader reader = selectIdCommand.ExecuteReader();
                            investmentItemId = int.Parse(reader[0].ToString());
                        }

                        _connection.Close();
                        _connection.Open();
                        commandString = $"INSERT INTO investment (investment_shares, investment_item_id, investing_account_id) VALUES ({investment.Shares}, {investmentItemId}, {accountId});";
                        using (SQLiteCommand insertCommand = new(commandString, _connection))
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    // get account info id
                    _connection.Close();
                    _connection.Open();
                    commandString = $"SELECT investing_account_info_id FROM investing_account WHERE investing_account_id={account.Id}";
                    using SQLiteCommand getIdCommand = new(commandString, _connection);
                    using SQLiteDataReader reader = getIdCommand.ExecuteReader();
                    accountInfoId = int.Parse(reader[0].ToString());

                    // update account info table
                    _connection.Close();
                    _connection.Open();
                    commandString = $"UPDATE investing_account_info SET investing_account_info_name='{account.Name}', investing_account_info_type={(int)account.Type} WHERE investing_account_info_id={accountInfoId}";
                    using SQLiteCommand updateNameCommand = new(commandString, _connection);
                    updateNameCommand.ExecuteNonQuery();

                    // add investments
                    foreach(Investment investment in account.Investments)
                    {
                        _connection.Close();
                        _connection.Open();
                        commandString = $"UPDATE investment SET investment_shares={investment.Shares} WHERE investment_id={investment.Id}";
                        using (SQLiteCommand updateCommand = new(commandString, _connection))
                        {
                            updateCommand.ExecuteNonQuery();
                        }

                        _connection.Close();
                        _connection.Open();
                        commandString = $"UPDATE investment_item SET investment_item_name='{investment.Name}', investment_item_symbol='{investment.Symbol}', investment_item_type={(int)investment.Type}, investment_item_stock_exchange={(int)investment.StockExchange} WHERE investment_item_id=(SELECT investment_item_id FROM investment WHERE investment_id={investment.Id})";
                        using (SQLiteCommand updateCommand = new(commandString, _connection))
                        {
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                }
                _connection.Close();
            }
        }
    }
}
