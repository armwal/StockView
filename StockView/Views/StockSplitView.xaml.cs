using StockView.Models;
using StockView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockView.Views
{
    /// <summary>
    /// Interaction logic for StockSplitView.xaml
    /// </summary>
    public partial class StockSplitView : Window
    {
        public StockSplitView(IEnumerable<Stock> availableStocks, Stock initial = null)
        {
            InitializeComponent();

            ((StockSplitViewModel)DataContext).Initialize(availableStocks, initial);
        }

        private void OnOKClick(object sender, EventArgs e)
        {
            if (DataContext is StockSplitViewModel vm)
            {
                vm.SelectedStock.Stock.PerformSplit(vm.Shares, vm.Price);
            }
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
