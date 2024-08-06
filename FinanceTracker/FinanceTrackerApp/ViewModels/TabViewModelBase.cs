using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using CsvHelper.Configuration;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public abstract partial class TabViewModelBase : ObservableObject
    {
        protected static readonly string s_appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FinanceTracker");
        private static readonly CsvConfiguration s_csvConfig = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            
        };
        [ObservableProperty]
        private DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

        public abstract void Closing();

        /// <summary>
        /// Sets data in csv file 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        protected static void SetData<T>(string filename, ObservableCollection<T> data)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
                File.SetAttributes(filename, File.GetAttributes(filename) | FileAttributes.Hidden);
                File.Encrypt(filename);
            }
            FileInfo myFile = new(filename);
            myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamWriter writer = new(filename);
            using CsvWriter csvWrite = new(writer, s_csvConfig);
            csvWrite.WriteRecords(data);
            myFile.Attributes |= FileAttributes.Hidden;
        }

        protected static void SetData<T, F>(string filename, ObservableCollection<T> data) where F : ClassMap<T>
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
                File.SetAttributes(filename, File.GetAttributes(filename) | FileAttributes.Hidden);
                File.Encrypt(filename);
            }
            FileInfo myFile = new(filename);
            myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamWriter writer = new(filename);
            using CsvWriter csvWrite = new(writer, s_csvConfig);
            csvWrite.Context.RegisterClassMap<F>();
            csvWrite.WriteRecords(data);
            myFile.Attributes |= FileAttributes.Hidden;
        }

        /// <summary>
        /// Gets data from csv file 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected static ObservableCollection<T> GetData<T>(string filename)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
                //File.SetAttributes(filename, File.GetAttributes(filename) | FileAttributes.Hidden);
                //File.Encrypt(filename);
            }
            FileInfo myFile = new(filename);
            //myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamReader reader = new(filename);
            using CsvReader csvRead = new(reader, s_csvConfig);
            IEnumerable<T> records = csvRead.GetRecords<T>();
            //myFile.Attributes |= FileAttributes.Hidden;
            return new ObservableCollection<T>(records);
        }

        protected static ObservableCollection<T> GetData<T, F>(string filename) where F : ClassMap<T>
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
                //File.SetAttributes(filename, File.GetAttributes(filename) | FileAttributes.Hidden);
                //File.Encrypt(filename);
            }
            FileInfo myFile = new(filename);
            //myFile.Attributes &= ~FileAttributes.Hidden;
            using StreamReader reader = new(filename);
            using CsvReader csvRead = new(reader, s_csvConfig);
            csvRead.Context.RegisterClassMap<F>();
            IEnumerable<T> records = csvRead.GetRecords<T>();
            //myFile.Attributes |= FileAttributes.Hidden;
            return new ObservableCollection<T>(records);
        }
    }
}
