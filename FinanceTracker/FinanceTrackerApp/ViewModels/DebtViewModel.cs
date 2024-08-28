using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTrackerApp.ViewModels
{
    public partial class DebtViewModel : TabViewModelBase
    {
        [ObservableProperty]
        private Controller _controller;

        public DebtViewModel(Controller controller)
        {
            Controller = controller;
        }

        public override void Closing()
        {

        }

        [RelayCommand]
        public override async Task Update()
        {

        }
    }
}
