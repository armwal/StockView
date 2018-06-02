using StockView.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using static StockView.Models.Transaction;

namespace StockViewTest
{
    public class TransactionTest
    {
        private const string fileName = @"../../output/trans.xml";

        public TransactionTest()
        {
            if (!Directory.Exists(@"../../output"))
            {
                Directory.CreateDirectory(@"../../output");
            }
        }

        [Fact]
        public void TestCreateBuy()
        {
            Transaction trans = Transaction.CreateBuyTransaction(42, 123.45M);

            Assert.Equal(enType.Buy, trans.Type);
            Assert.Equal(42, trans.Shares);
            Assert.Equal(123.45M, trans.TotalPrice);
        }

        [Fact]
        public void TestCreateSell()
        {
            Transaction trans = Transaction.CreateSellTransaction(42, 123.45M);

            Assert.Equal(enType.Sell, trans.Type);
            Assert.Equal(42, trans.Shares);
            Assert.Equal(123.45M, trans.TotalPrice);
        }

        [Fact]
        public void TestWriteReadBuy()
        {
            Transaction trans = Transaction.CreateBuyTransaction(42, 123.45M);

            XDocument doc = new XDocument();
            doc.Add(trans.Write());

            doc.Save(fileName);

            XDocument imp = XDocument.Load(fileName);
            var el = imp.Element("Transaction");

            Transaction imported = Transaction.FromXml(el);

            Assert.Equal(trans.Shares, imported.Shares);
            Assert.Equal(trans.PricePerShare, imported.PricePerShare);
            Assert.Equal(trans.TotalPrice, imported.TotalPrice);
            Assert.Equal(trans.Type, imported.Type);
            Assert.Equal(trans.Date, imported.Date);
        }

        [Fact]
        public void TestWriteReadSell()
        {
            Transaction trans = Transaction.CreateSellTransaction(42, 123.45M);

            XDocument doc = new XDocument();
            doc.Add(trans.Write());

            doc.Save(fileName);

            XDocument imp = XDocument.Load(fileName);
            var el = imp.Element("Transaction");

            Transaction imported = Transaction.FromXml(el);

            Assert.Equal(trans.Shares, imported.Shares);
            Assert.Equal(trans.PricePerShare, imported.PricePerShare);
            Assert.Equal(trans.TotalPrice, imported.TotalPrice);
            Assert.Equal(trans.Type, imported.Type);
            Assert.Equal(trans.Date, imported.Date);
        }
    }
}
