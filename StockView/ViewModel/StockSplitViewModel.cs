using GalaSoft.MvvmLight;
using StockView.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.ViewModel
{
    class StockSplitViewModel : ViewModelBase
    {
        private StockViewModel selectedStock;
        private int shares;
        private decimal price;

        public ObservableCollection<StockViewModel> AvailableStocks { get; }
        public StockViewModel SelectedStock
        {
            get => selectedStock;
            set
            {
                selectedStock = value;
                Shares = selectedStock.Shares;
                Price = selectedStock.Stock.CurrentPricePerShare;
                RaisePropertyChanged(nameof(SelectedStock));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int Shares
        {
            get => shares;
            set
            {
                shares = value;
                RaisePropertyChanged(nameof(Shares));
            }
        }

        public decimal Price
        {
            get => price;
            set
            {
                price = value;
                RaisePropertyChanged(nameof(Price));
            }
        }

        public bool IsValid
        {
            get
            {
                return SelectedStock != null;
            }
        }

        public StockSplitViewModel()
        {
            AvailableStocks = new ObservableCollection<StockViewModel>();
        }

        public void Initialize(IEnumerable<Stock> availableStocks, Stock initial = null)
        {
            AvailableStocks.Clear();
            foreach (var stock in availableStocks)
            {
                AvailableStocks.Add(new StockViewModel(stock));
            }
            if (initial != null)
            {
                SelectedStock = AvailableStocks.FirstOrDefault(x => x.Title == initial.Title);
            }
        }
    }
}
