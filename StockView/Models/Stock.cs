using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.Models
{
    public class Stock
    {
        private decimal currentPrice;

        public string Title { get; }
        public string WKN { get; }
        public int Shares { get; private set; }
        public decimal BuyPricePerShare { get; private set; }
        public decimal CurrentPricePerShare
        {
            get { return currentPrice; }
            set
            {
                currentPrice = value;
                EvtUpdate?.Invoke(this, new UpdateEventArgs(this));
            }
        }
        public decimal RealizedRevenue { get; private set; }

        public class UpdateEventArgs : EventArgs
        {
            public Stock Stock { get; }

            public UpdateEventArgs(Stock stock)
            {
                Stock = stock;
            }
        }

        public event EventHandler<UpdateEventArgs> EvtUpdate;

        public Stock(string title, string wkn)
        {
            Title = title;
            WKN = wkn;
        }

        public void Buy(int shares, decimal totalCost)
        {
            BuyPricePerShare = (BuyPricePerShare * Shares + totalCost) / (Shares + shares);
            Shares += shares;

            EvtUpdate?.Invoke(this, new UpdateEventArgs(this));
        }

        public void Sell(int shares, decimal totalPrice)
        {
            if (shares > Shares)
            {
                throw new ArgumentException("Can't sell more than you have!");
            }

            Shares -= shares;

            RealizedRevenue += totalPrice - (shares * BuyPricePerShare);

            EvtUpdate?.Invoke(this, new UpdateEventArgs(this));
        }
    }
}
