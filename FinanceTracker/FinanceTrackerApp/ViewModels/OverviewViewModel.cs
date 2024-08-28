using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class OverviewViewModel : TabViewModelBase
    {
        [ObservableProperty]
        private Controller _data;

        public OverviewViewModel(Controller data)
        {
            Data = data;
        }

        public override void Closing()
        {
            throw new NotImplementedException();
        }

        public override Task Update()
        {
            throw new NotImplementedException();
        }
    }
}
