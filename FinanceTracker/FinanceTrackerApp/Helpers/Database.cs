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
    public static class Database
    {
        private static readonly string s_appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker");
        private static readonly string _filename = Path.Combine(s_appDataFolder,"FinanceTracker.sqlite");
        private static readonly string _connectionString = $"Data Source={_filename};";
        private static readonly string _dateFormat = "yyyy/MM/dd";

        public static void CreateDatabase()
        {
            string commandString;
            if (!Directory.Exists(s_appDataFolder))
            {
                Directory.CreateDirectory(s_appDataFolder);
            }
            // create database and set up connection
            if (!File.Exists(_filename))
            {
                SQLiteConnection.CreateFile(_filename);
            }
            using SQLiteConnection connection = new(_connectionString);

            // create account tables
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS account_info (account_info_id INTEGER PRIMARY KEY, account_name VARCHAR(100));";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS account (account_id INTEGER PRIMARY KEY, account_info_id INTEGER NOT NULL, account_total REAL, account_interest REAL, account_date VARCHAR(10), FOREIGN KEY (account_info_id) REFERENCES account_info (account_info_id));";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();

            // create investing account tables
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investing_account_info (investing_account_info_id INTEGER PRIMARY KEY, investing_account_info_name VARCHAR(100), investing_account_info_type INTEGER);";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investing_account (investing_account_id INTEGER PRIMARY KEY, investing_account_info_id INTEGER, investing_account_date VARCHAR(10), FOREIGN KEY (investing_account_info_id) REFERENCES investing_account_info (investing_account_info_id));";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();

            // creates investment table
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investment (investment_id INTEGER PRIMARY KEY, investment_shares INTEGER, investment_item_id INTEGER, investing_account_id INTEGER, FOREIGN KEY (investment_item_id) REFERENCES investment_item (investment_item_id), FOREIGN KEY (investing_account_id) REFERENCES investing_account (investing_account_id));";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();

            // create investment item tables
            connection.Open();
            commandString = $"CREATE TABLE IF NOT EXISTS investment_item (investment_item_id INTEGER PRIMARY KEY, investment_item_name VARCHAR(100), investment_item_symbol VARCHAR(100), investment_item_type INTEGER, investment_item_stock_exchange INTEGER);";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        private static int GetInsertedRowId(SQLiteConnection connection)
        {
            int rowId = -1;
            string commandString = $"SELECT last_insert_rowid();";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    rowId = int.Parse(reader[0].ToString());
                }

            }
            return rowId;
        }

        private static bool RowExist(SQLiteConnection connection, string table, int id)
        {
            string commandString = $"SELECT exists(SELECT 1 FROM {table} WHERE {table}_id={id}) AS row_exists;";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                return reader.Read() && int.Parse(reader[0].ToString()) != 0;
            }
        }

        private static bool RowExist(SQLiteConnection connection, string table, int id, DateOnly date)
        {
            string commandString = $"SELECT exists(SELECT 1 FROM {table} WHERE {table}_id={id} AND {table}_date='{date.ToString(_dateFormat)}') AS row_exists;";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                return reader.Read() && int.Parse(reader[0].ToString()) != 0;
            }
        }

        public static void DeleteAccount(Account account)
        {
            string commandString;
            using SQLiteConnection connection = new(_connectionString);
            connection.Open();
            commandString = $"DELETE FROM account_info WHERE account_info_id=(SELECT account_info_id FROM account WHERE account_id={account.Id})";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
            connection.Open();
            commandString = $"DELETE FROM account WHERE account_id={account.Id}";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static ObservableCollection<Account> GetAccounts(DateOnly date)
        {
            string commandString;
            using SQLiteConnection connection = new(_connectionString);
            ObservableCollection<Account> accounts = new();

            connection.Open();
            commandString = $"SELECT * FROM account_info";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    accounts.Add(new()
                    {
                        InfoId = int.Parse(reader["account_info_id"].ToString()),
                        Name = reader["account_name"].ToString(),
                        Date = date
                    });
                }
            }
            
            foreach (Account account in accounts)
            {
                connection.Close();
                connection.Open();
                commandString = $"SELECT account_id, account_total, account_interest, account_date FROM account INNER JOIN account_info ON account_info.account_info_id=account.account_info_id WHERE account.account_info_id={account.InfoId} AND account_date<='{date.ToString(_dateFormat)}' ORDER BY account_date DESC";
                using (SQLiteCommand command = new(commandString, connection))
                {
                    using SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        account.Id = int.Parse(reader["account_id"].ToString());
                        account.Total = float.Parse(reader["account_total"].ToString());
                        account.Interest = float.Parse(reader["account_interest"].ToString());
                    }
                }
            }
            connection.Close();

            /*connection.Open();
            commandString = $"SELECT account.account_info_id, account_id, account_name, account_total, account_interest, account_date FROM account INNER JOIN account_info ON account_info.account_info_id=account.account_info_id WHERE account_date='{date.ToString(_dateFormat)}'";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Add(new()
                    {
                        Id = int.Parse(reader["account_id"].ToString()),
                        InfoId = int.Parse(reader["account_info_id"].ToString()),
                        Name = reader["account_name"].ToString(),
                        Total = float.Parse(reader["account_total"].ToString()),
                        Interest = float.Parse(reader["account_interest"].ToString()),
                        Date = DateOnly.ParseExact(reader["account_date"].ToString(), _dateFormat) // change to be current date regardless
                    });
                }
            }
            connection.Close();*/
            return accounts;
        }

        public static void SetAccounts(ObservableCollection<Account> accounts)
        {
            string commandString;
            bool newAccount;
            bool newAccountInfo;
            using SQLiteConnection connection = new(_connectionString);
            // temp
            /*DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            for (int i = 0; i < 10; i++)
            {
                
            }*/
            // temp end
            foreach (Account account in accounts)
            {
                //account.Date = date.AddDays(-1 * i);
                connection.Open();
                newAccountInfo = !RowExist(connection, "account_info", account.InfoId);
                if (newAccountInfo)
                {
                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO account_info (account_name) VALUES ('{account.Name}');";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    account.InfoId = GetInsertedRowId(connection);
                }
                else
                {
                    connection.Close();
                    connection.Open();
                    commandString = $"UPDATE account_info SET account_name='{account.Name}' WHERE account_info_id={account.InfoId}";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
                connection.Open();
                newAccount = !RowExist(connection, "account", account.Id, account.Date);

                if (newAccount)
                {
                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO account (account_info_id, account_total, account_interest, account_date) VALUES ('{account.InfoId}', {account.Total}, {account.Interest}, '{account.Date.ToString(_dateFormat)}');";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    account.Id = GetInsertedRowId(connection);
                }
                else
                {

                    connection.Close();
                    connection.Open();
                    commandString = $"UPDATE account SET account_total={account.Total}, account_interest={account.Interest} WHERE account_id={account.Id}";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public static void DeleteInvestingAccount(InvestingAccount account)
        {
            using SQLiteConnection connection = new(_connectionString);
            foreach (Investment investment in account.Investments)
            {
                DeleteInvestment(investment);
            }
            connection.Open();
            string commandString = $"DELETE FROM investing_account_info WHERE investing_account_info_id=(SELECT investing_account_info_id FROM investing_account WHERE investing_account_id={account.Id})";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
            connection.Open();
            commandString = $"DELETE FROM investing_account WHERE investing_account_info_id={account.Id}";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static void DeleteInvestment(Investment investment)
        {
            using SQLiteConnection connection = new(_connectionString);
            connection.Open();
            string commandString = $"DELETE FROM investment_item WHERE investment_item_id=(SELECT investment_item_id FROM investment WHERE investment_id={investment.Id})";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
            connection.Open();
            commandString = $"DELETE FROM investment WHERE investment_id={investment.Id}";
            using (SQLiteCommand command = new(commandString, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static ObservableCollection<InvestingAccount> GetInvestingAccounts(DateOnly date)
        {
            using SQLiteConnection connection = new(_connectionString);
            ObservableCollection<InvestingAccount> accounts = new();
            connection.Open();
            string commandString = $"SELECT investing_account_id, investing_account_info_name, investing_account_info_type, investing_account_date FROM investing_account INNER JOIN investing_account_info ON investing_account_info.investing_account_info_id=investing_account.investing_account_info_id WHERE investing_account_date='{date.ToString(_dateFormat)}'";
            using (SQLiteCommand command = new(commandString, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Add(new()
                    {
                        Id = int.Parse(reader["investing_account_id"].ToString()),
                        Name = reader["investing_account_info_name"].ToString(),
                        Type = (InvestingAccountType)Enum.Parse(typeof(InvestingAccountType), reader["investing_account_info_type"].ToString()),
                        Date = DateOnly.ParseExact(reader["investing_account_date"].ToString(), _dateFormat)
                    });
                }
            }
            
           
            foreach(InvestingAccount account in accounts)
            {
                connection.Close();
                connection.Open();
                commandString = $"SELECT investment_id, investment_shares, investment_item_name, investment_item_symbol, investment_item_type, investment_item_stock_exchange FROM investment INNER JOIN investment_item ON investment.investment_item_id=investment_item.investment_item_id WHERE investing_account_id={account.Id}";
                using SQLiteCommand command = new(commandString, connection);
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

            connection.Close();
            return accounts;
        }

        private static void SetInvestments(SQLiteConnection connection, InvestingAccount account)
        {
            string commandString;
            bool newInvestment;
            foreach (Investment investment in account.Investments)
            {
                connection.Close();
                connection.Open();
                newInvestment = !RowExist(connection, "investment", investment.Id);

                if (newInvestment)
                {
                    int investmentItemId;
                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO investment_item (investment_item_name, investment_item_symbol, investment_item_type, investment_item_stock_exchange) VALUES ('{investment.Name}', '{investment.Symbol}', {(int)investment.Type}, {(int)investment.StockExchange});";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    investmentItemId = GetInsertedRowId(connection);

                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO investment (investment_shares, investment_item_id, investing_account_id) VALUES ({investment.Shares}, {investmentItemId}, {account.Id});";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    connection.Close();
                    connection.Open();
                    commandString = $"UPDATE investment SET investment_shares={investment.Shares} WHERE investment_id={investment.Id}";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                    connection.Open();
                    commandString = $"UPDATE investment_item SET investment_item_name='{investment.Name}', investment_item_symbol='{investment.Symbol}', investment_item_type={(int)investment.Type}, investment_item_stock_exchange={(int)investment.StockExchange} WHERE investment_item_id=(SELECT investment_item_id FROM investment WHERE investment_id={investment.Id})";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SetInvestingAccounts(ObservableCollection<InvestingAccount> accounts)
        {
            string commandString;
            bool newInvestmentAccount;
            using SQLiteConnection connection = new(_connectionString);
            foreach (InvestingAccount account in accounts)
            {
                int accountInfoId = -1;

                // Checks if investment exists
                connection.Open();
                newInvestmentAccount = !RowExist(connection, "investing_account", account.Id);
                
                if (newInvestmentAccount)
                {
                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO investing_account_info (investing_account_info_name, investing_account_info_type) VALUES ('{account.Name}', {(int)account.Type});";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    accountInfoId = GetInsertedRowId(connection);
                    

                    connection.Close();
                    connection.Open();
                    commandString = $"INSERT INTO investing_account (investing_account_info_id, investing_account_date) VALUES ('{accountInfoId}', '{account.Date.ToString(_dateFormat)}');";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    account.Id = GetInsertedRowId(connection);

                    SetInvestments(connection, account);
                }
                else
                {
                    // get account info id
                    connection.Close();
                    connection.Open();
                    commandString = $"SELECT investing_account_info_id FROM investing_account WHERE investing_account_id={account.Id}";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            accountInfoId = int.Parse(reader[0].ToString());
                        }
                    }

                    // update account info table
                    connection.Close();
                    connection.Open();
                    commandString = $"UPDATE investing_account_info SET investing_account_info_name='{account.Name}', investing_account_info_type={(int)account.Type} WHERE investing_account_info_id={accountInfoId}";
                    using (SQLiteCommand command = new(commandString, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    SetInvestments(connection,account);
                }
                connection.Close();
            }
        }
    }
}
