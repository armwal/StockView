using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StockView.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.ViewModel
{
    class StockSellViewModel : ViewModelBase
    {
        private int shares;
        private Stock selected;
        private decimal price;

        public ObservableCollection<Stock> AvailableStocks { get; }
        public Stock SelectedStock
        {
            get { return selected; }
            set
            {
                selected = value;
                RaisePropertyChanged(nameof(SelectedStock));
                RaisePropertyChanged(nameof(CanSell));
            }
        }
        public int Shares
        {
            get { return shares; }
            set
            {
                shares = value;
                RaisePropertyChanged(nameof(Shares));
                RaisePropertyChanged(nameof(CanSell));
            }
        }
        public decimal PricePerShare
        {
            get { return price; }
            set
            {
                price = value;
                RaisePropertyChanged(nameof(PricePerShare));
                RaisePropertyChanged(nameof(CanSell));
            }
        }
        public bool CanSell
        {
            get
            {
                return SelectedStock != null && Shares > 0 && PricePerShare >= 0;
            }
        }
        public ICommand CmdSell { get; set; }

        public StockSellViewModel()
        {
            AvailableStocks = new ObservableCollection<Stock>();
            CmdSell = new RelayCommand(CmdSellExecute);
        }

        private void CmdSellExecute()
        {
            if (CanSell)
            {
                SelectedStock.Sell(Shares, Shares * PricePerShare);
            }
        }

        public void Initialize(IEnumerable<Stock> availableStocks, Stock initial = null)
        {
            foreach (var stock in availableStocks)
            {
                AvailableStocks.Add(stock);
            }
            SelectedStock = initial;
        }
    }
}
