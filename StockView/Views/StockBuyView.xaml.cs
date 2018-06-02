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
    /// Interaction logic for StockBuyView.xaml
    /// </summary>
    public partial class StockBuyView : Window
    {
        public StockBuyView(IEnumerable<Stock> availableStocks, Stock initial = null)
        {
            InitializeComponent();

            ((StockBuyViewModel)DataContext).Initialize(availableStocks, initial);
        }

        private void OnBuyClick(object sender, EventArgs e)
        {
            ((StockBuyViewModel)DataContext).CmdBuy.Execute(null);
            Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
