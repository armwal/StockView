using GalaSoft.MvvmLight;
using StockView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.ViewModel
{
    class StockAddViewModel : ViewModelBase
    {
        private List<Stock> stocks;
        private string title;
        private string wkn;
        private int shares;
        private decimal pricePerShare;
        private DateTime date;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public string WKN
        {
            get
            {
                return wkn;
            }
            set
            {
                wkn = value;
                RaisePropertyChanged(nameof(WKN));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int Shares
        {
            get
            {
                return shares;
            }
            set
            {
                shares = value;
                RaisePropertyChanged(nameof(Shares));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal PricePerShare
        {
            get
            {
                return pricePerShare;
            }
            set
            {
                pricePerShare = value;
                RaisePropertyChanged(nameof(PricePerShare));
                RaisePropertyChanged(nameof(IsValid));
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

        public bool IsValid
        {
            get
            {
                return Check();
            }
        }

        public StockAddViewModel()
        {
            stocks = new List<Stock>();
            date = DateTime.Now;
        }

        public void Initialize(List<Stock> availableStocks)
        {
            stocks = availableStocks;
        }

        public Stock CreateStock()
        {
            if (IsValid)
            {
                Stock newStock = new Stock(Title, WKN);
                if (Shares > 0)
                {
                    newStock.Buy(Shares, PricePerShare * Shares, Date);
                }
                newStock.CurrentPricePerShare = PricePerShare;

                return newStock;
            }
            return null;
        }

        private bool Check()
        {
            if (string.IsNullOrEmpty(Title) || Shares < 0 || PricePerShare < 0)
            {
                return false;
            }
            if (stocks.Any(x => x.Title == Title))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(WKN))
            {
                if (stocks.Any(x => x.WKN == WKN))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
