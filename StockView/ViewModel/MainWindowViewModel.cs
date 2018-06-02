using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using StockView.Models;
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

        public ObservableCollection<StockViewModel> Stocks { get; set; }

        public string TotalBuyPrice
        {
            get
            {
                return ToCurrency(Stocks.Sum(x => x.Stock.BuyPricePerShare * x.Stock.Shares));
            }
        }

        public string TotalSellPrice
        {
            get
            {
                return ToCurrency(Stocks.Sum(x => x.Stock.CurrentPricePerShare * x.Stock.Shares));
            }
        }

        public ICommand CmdOpen { get; set; }
        public ICommand CmdBuy { get; set; }
        public ICommand CmdSell { get; set; }



        public MainWindowViewModel()
        {
            Stocks = new ObservableCollection<StockViewModel>();
            CmdOpen = new RelayCommand(CmdOpenExecute);
            CmdBuy = new RelayCommand(CmdBuyExecute);
            CmdSell = new RelayCommand(CmdSellExecute);
        }

        private void CmdSellExecute()
        {
            throw new NotImplementedException();
        }

        private void CmdBuyExecute()
        {
            throw new NotImplementedException();
        }

        private void CmdOpenExecute()
        {
            if (Stocks.Count > 0)
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
