using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockView.Models
{
    public class Stock
    {
        private const int writeVersion = 1;
        private const string name = "Stock";

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
        public List<Transaction> Transactions { get; }

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
            Transactions = new List<Transaction>();
        }

        public void Buy(int shares, decimal totalCost)
        {
            BuyPricePerShare = (BuyPricePerShare * Shares + totalCost) / (Shares + shares);
            Shares += shares;

            Transactions.Add(Transaction.CreateBuyTransaction(shares, totalCost));

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

            Transactions.Add(Transaction.CreateSellTransaction(shares, totalPrice)); 

            EvtUpdate?.Invoke(this, new UpdateEventArgs(this));
        }

        public void PerformSplit(int newShares, decimal newPrice)
        {
            BuyPricePerShare *= (decimal)Shares / newShares;
            Shares = newShares;
            CurrentPricePerShare = newPrice;

            EvtUpdate?.Invoke(this, new UpdateEventArgs(this));
        }

        public XElement Write()
        {
            XElement element = new XElement(name);
            element.Add(new XAttribute("Version", writeVersion));
            element.Add(new XElement("Title", Title));
            element.Add(new XElement("WKN", WKN));
            element.Add(new XElement("Shares", Shares));
            element.Add(new XElement("BuyPricePerShare", BuyPricePerShare.ToString(CultureInfo.InvariantCulture)));
            element.Add(new XElement("CurrentPricePerShare", CurrentPricePerShare.ToString(CultureInfo.InvariantCulture)));
            element.Add(new XElement("RealizedRevenue", RealizedRevenue.ToString(CultureInfo.InvariantCulture)));

            XElement transactions = new XElement("Transactions");
            transactions.Add(new XAttribute("Count", Transactions.Count));
            foreach (var trans in Transactions)
            {
                transactions.Add(trans.Write());
            }

            element.Add(transactions);

            return element;
        }

        public static Stock FromXml(XElement element)
        {
            if (element.Name != name)
            {
                throw new ArgumentException("Cannot read stock from XML");
            }

            Stock stock = new Stock(element.Element("Title").Value, element.Element("WKN").Value);
            stock.Shares = int.Parse(element.Element("Shares").Value);
            stock.BuyPricePerShare = decimal.Parse(element.Element("BuyPricePerShare").Value, CultureInfo.InvariantCulture);
            stock.CurrentPricePerShare = decimal.Parse(element.Element("CurrentPricePerShare").Value, CultureInfo.InvariantCulture);
            stock.RealizedRevenue = decimal.Parse(element.Element("RealizedRevenue").Value, CultureInfo.InvariantCulture);

            var transactions = element.Element("Transactions");
            int count = int.Parse(transactions.Attribute("Count").Value);
            foreach (var el in transactions.Elements())
            {
                Transaction trans = Transaction.FromXml(el);
                stock.Transactions.Add(trans);
            }

            return stock;
        }
    }
}
