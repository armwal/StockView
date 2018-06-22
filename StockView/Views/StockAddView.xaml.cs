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
    /// Interaction logic for StockAddView.xaml
    /// </summary>
    public partial class StockAddView : Window
    {
        public Stock NewStock { get; private set; }

        public StockAddView(List<Stock> stocks)
        {
            InitializeComponent();

            ((StockAddViewModel)DataContext).Initialize(stocks);
            TitleBox.Focus();
        }

        private void OnAddClick(object sender, EventArgs e)
        {
            NewStock = ((StockAddViewModel)DataContext).CreateStock();
            Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
