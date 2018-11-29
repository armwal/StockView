using GalaSoft.MvvmLight;
using StockView.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StockView.ViewModel
{
    public class StockViewModel : ViewModelBase
    {
        private static SolidColorBrush plusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DD000000"));
        private static SolidColorBrush minusBrush = Brushes.Red;

        public Stock Stock { get; }

        public string Title
        {
            get
            {
                return Stock.Title;
            }
        }

        public string WKN
        {
            get
            {
                return Stock.WKN;
            }
        }

        public int Shares
        {
            get
            {
                return Stock.Shares;
            }
        }

        public string BuyPricePerShare
        {
            get
            {
                return ToCurrency(Stock.BuyPricePerShare);
            }
        }

        public string CurrentPricePerShare
        {
            get
            {
                return ToCurrency(Stock.CurrentPricePerShare);
            }
            set
            {
                bool success = decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("de-DE"), out decimal price);
                if (success)
                {
                    Stock.CurrentPricePerShare = price;
                }
                else
                {
                    Stock.CurrentPricePerShare = Stock.CurrentPricePerShare;
                    MessageBox.Show("Fehler beim Interpretieren des aktuellen Kurses: Muss Währung sein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                RaisePropertyChanged(nameof(CurrentPricePerShare));
            }
        }

        public string BuyPriceTotal
        {
            get
            {
                return ToCurrency(Stock.BuyPricePerShare * Stock.Shares);
            }
        }

        public string SellPriceTotal
        {
            get
            {
                return ToCurrency(Stock.CurrentPricePerShare * Stock.Shares);
            }
        }

        public string PossibleRevenueTotal
        {
            get
            {
                return ToCurrency(Stock.CurrentPricePerShare * Stock.Shares - Stock.BuyPricePerShare * Stock.Shares);
            }
        }

        public Brush RevenueBrush
        {
            get
            {
                if (Stock.CurrentPricePerShare >= Stock.BuyPricePerShare)
                {
                    return plusBrush;
                }
                return minusBrush;
            }
        }

        public string RealizedRevenueTotal
        {
            get
            {
                return ToCurrency(Stock.RealizedRevenue);
            }
        }

        public Brush RealizedRevenueBrush
        {
            get
            {
                if (Stock.RealizedRevenue >= 0)
                {
                    return plusBrush;
                }
                return minusBrush;
            }
        }

        public string PossibleRevenuePercentage
        {
            get
            {
                var value = Stock.CurrentPricePerShare * Stock.Shares - Stock.BuyPricePerShare * Stock.Shares;
                value /= Stock.BuyPricePerShare * Stock.Shares;
                value *= 100;
                value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
                return value.ToString("G", CultureInfo.CreateSpecificCulture("de-DE")) + " %";
            }
        }

        public DateTime BuyDate
        {
            get
            {
                return Stock.BuyDate;
            }
            set
            {
                Stock.BuyDate = value;
                RaisePropertyChanged(nameof(BuyDate));
            }
        }

        public event EventHandler EvtUpdate;

        public StockViewModel(Stock stock)
        {
            Stock = stock;
            stock.EvtUpdate += OnStockUpdate;
        }

        private void OnStockUpdate(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Shares));
            RaisePropertyChanged(nameof(BuyPricePerShare));
            RaisePropertyChanged(nameof(CurrentPricePerShare));
            RaisePropertyChanged(nameof(BuyPriceTotal));
            RaisePropertyChanged(nameof(SellPriceTotal));
            RaisePropertyChanged(nameof(PossibleRevenueTotal));
            RaisePropertyChanged(nameof(RevenueBrush));
            RaisePropertyChanged(nameof(RealizedRevenueBrush));
            RaisePropertyChanged(nameof(PossibleRevenuePercentage));
            RaisePropertyChanged(nameof(BuyDate));

            EvtUpdate?.Invoke(this, EventArgs.Empty);
        }

        private string ToCurrency(decimal value)
        {
            return value.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
