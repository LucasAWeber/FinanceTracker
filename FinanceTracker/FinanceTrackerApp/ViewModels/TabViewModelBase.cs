using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public abstract void Closing();
    }
}
