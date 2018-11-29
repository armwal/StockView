using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockView.Models
{
    public class Transaction
    {
        private const int writeVersion = 1;
        private const string name = "Transaction";

        public enum enType
        { 
            Buy,
            Sell
        }

        public DateTime Date { get; set; }
        public enType Type { get; set; }
        public int Shares { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerShare
        {
            get
            {
                return Shares == 0 ? 0 : TotalPrice / Shares;
            }
        }

        public static Transaction CreateBuyTransaction(int shares, decimal totalPrice, DateTime date)
        {
            Transaction trans = new Transaction();
            trans.Date = date;
            trans.Type = enType.Buy;
            trans.Shares = shares;
            trans.TotalPrice = totalPrice;

            return trans;
        }

        public static Transaction CreateSellTransaction(int shares, decimal totalPrice, DateTime date)
        {
            Transaction trans = new Transaction();
            trans.Date = date;
            trans.Type = enType.Sell;
            trans.Shares = shares;
            trans.TotalPrice = totalPrice;

            return trans;
        }

        public XElement Write()
        {
            XElement element = new XElement(name);
            element.Add(new XAttribute("Version", writeVersion));
            element.Add(new XElement("Date", Date.ToString("o", CultureInfo.CreateSpecificCulture("de-DE"))));
            element.Add(new XElement("Type", Type.ToString()));
            element.Add(new XElement("Shares", Shares));
            element.Add(new XElement("Total", TotalPrice.ToString(CultureInfo.InvariantCulture)));

            return element;
        }

        public static Transaction FromXml(XElement element)
        {
            if (element.Name != name)
            {
                throw new ArgumentException("Cannot read transaction!");
            }

            Transaction trans = new Transaction();
            trans.Date = DateTime.Parse(element.Element("Date").Value, CultureInfo.CreateSpecificCulture("de-DE"));
            trans.Type = (enType)Enum.Parse(typeof(enType), element.Element("Type").Value);
            trans.Shares = int.Parse(element.Element("Shares").Value);
            trans.TotalPrice = decimal.Parse(element.Element("Total").Value, CultureInfo.InvariantCulture);

            return trans;
        }
    }
}
