using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using StockView.Models;
using StockView.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string fileName;
        private List<Stock> stocks;

        public ObservableCollection<StockViewModel> StockVms { get; set; }

        public string TotalBuyPrice
        {
            get
            {
                return ToCurrency(stocks.Sum(x => x.BuyPricePerShare * x.Shares));
            }
        }

        public string TotalSellPrice
        {
            get
            {
                return ToCurrency(stocks.Sum(x => x.CurrentPricePerShare * x.Shares));
            }
        }

        public ICommand CmdOpen { get; set; }
        public ICommand CmdAddNew { get; set; }
        public ICommand CmdBuy { get; set; }
        public ICommand CmdSell { get; set; }



        public MainWindowViewModel()
        {
            stocks = new List<Stock>();
            StockVms = new ObservableCollection<StockViewModel>();
            CmdOpen = new RelayCommand(CmdOpenExecute);
            CmdBuy = new RelayCommand(CmdBuyExecute);
            CmdSell = new RelayCommand(CmdSellExecute);
            CmdAddNew = new RelayCommand(CmdAddNewExecute);
        }

        private void CmdAddNewExecute()
        {
            StockAddView addDlg = new StockAddView(stocks);
            addDlg.ShowDialog();

            if (addDlg.NewStock != null)
            {
                stocks.Add(addDlg.NewStock);
                StockVms.Add(new StockViewModel(addDlg.NewStock));
            }
        }

        private void CmdSellExecute()
        {
            StockSellView sellDlg = new StockSellView(stocks);
            sellDlg.ShowDialog();
        }

        private void CmdBuyExecute()
        {
            StockBuyView buyDlg = new StockBuyView(stocks);
            buyDlg.ShowDialog();
        }

        private void CmdOpenExecute()
        {
            if (StockVms.Count > 0)
            {
                SaveToFile();
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Lade Depotübersicht";
            dlg.Filter = "Depots (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                LoadFromFile(dlg.FileName);
            }
        }

        public void LoadFromFile(string loadFileName)
        {
            fileName = loadFileName;
        }

        public void SaveToFile()
        {
            if (!string.IsNullOrEmpty(fileName))
            {

            }
        }

        private string ToCurrency(decimal value)
        {
            return value.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
        }
    }
}
