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
    class StockBuyViewModel : ViewModelBase
    {
        private int shares;
        private Stock selected;
        private decimal price;
        private DateTime date;

        public ObservableCollection<Stock> AvailableStocks { get; }
        public Stock SelectedStock
        {
            get { return selected; }
            set
            {
                selected = value;
                RaisePropertyChanged(nameof(SelectedStock));
                RaisePropertyChanged(nameof(CanBuy));
            }
        }
        public int Shares
        {
            get { return shares; }
            set
            {
                shares = value;
                RaisePropertyChanged(nameof(Shares));
                RaisePropertyChanged(nameof(CanBuy));
            }
        }
        public decimal PricePerShare
        {
            get { return price; }
            set
            {
                price = value;
                RaisePropertyChanged(nameof(PricePerShare));
                RaisePropertyChanged(nameof(CanBuy));
            }
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public bool CanBuy
        {
            get
            {
                return SelectedStock != null && Shares > 0 && PricePerShare >= 0;
            }
        }
        public ICommand CmdBuy { get; set; }

        public StockBuyViewModel()
        {
            AvailableStocks = new ObservableCollection<Stock>();
            CmdBuy = new RelayCommand(CmdBuyExecute);
            date = DateTime.Now;
        }

        private void CmdBuyExecute()
        {
            if (CanBuy)
            {
                SelectedStock.Buy(Shares, Shares * PricePerShare, Date);
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
