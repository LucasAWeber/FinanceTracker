using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTrackerApp.Helpers
{
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object BindingDataContext
        {
            get { return (object)GetValue(BindingDataContextProperty); }
            set { SetValue(BindingDataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingDataContextProperty =
            DependencyProperty.Register("BindingDataContext", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
