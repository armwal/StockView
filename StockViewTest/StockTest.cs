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
    public class StockTest
    {
        private Stock updatedStock;
        private const string fileName = @"../../output/stock.xml";

        public StockTest()
        {
            if (!Directory.Exists(@"../../output"))
            {
                Directory.CreateDirectory(@"../../output");
            }
        }

        [Fact]
        public void TestCtor()
        {
            Stock stock = new Stock("Test", "TestWKN");

            Assert.Equal("Test", stock.Title);
            Assert.Equal("TestWKN", stock.WKN);
            Assert.Equal(0, stock.Shares);
            Assert.Equal(0, stock.BuyPricePerShare);
            Assert.Equal(0, stock.CurrentPricePerShare);
            Assert.Equal(0, stock.RealizedRevenue);
            Assert.Equal(default(DateTime), stock.BuyDate);
        }

        [Fact]
        public void TestBuy()
        {
            Stock stock = new Stock("Test", "TestWKN");

            DateTime testDate = new DateTime(2018, 3, 12);

            stock.Buy(10, 123.45M, testDate);

            Assert.Equal(10, stock.Shares);
            Assert.Equal(12.345M, stock.BuyPricePerShare);
            Assert.Equal(testDate, stock.BuyDate);
        }

        [Fact]
        public void TestBuyMultiple()
        {
            Stock stock = new Stock("Test", "TestWKN");

            DateTime testDate1 = new DateTime(2018, 3, 12);
            DateTime testDate2 = new DateTime(2018, 3, 13);
            DateTime testDate3 = new DateTime(2018, 3, 14);

            stock.Buy(10, 100M, testDate1);
            stock.Buy(10, 200M, testDate2);
            stock.Buy(10, 300M, testDate3);

            Assert.Equal(30, stock.Shares);
            Assert.Equal(20M, stock.BuyPricePerShare);
            Assert.Equal(testDate1, stock.BuyDate);
        }

        [Fact]
        public void TestBuySendsUpdate()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.EvtUpdate += OnStockUpdate;

            DateTime testDate = new DateTime(2018, 3, 12);

            stock.Buy(10, 123.45M, testDate);

            Assert.Same(stock, updatedStock);
        }

        [Fact]
        public void TestSetCurrentPriceSendsUpdate()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.EvtUpdate += OnStockUpdate;

            stock.CurrentPricePerShare = 100M;

            Assert.Same(stock, updatedStock);
        }

        [Fact]
        public void TestSell()
        {
            Stock stock = new Stock("Test", "TestWKN");

            stock.Buy(10, 123.45M, DateTime.Now);

            stock.Sell(4, 345.67M, DateTime.Now);

            Assert.Equal(6, stock.Shares);
            Assert.Equal(345.67M - (4 * 12.345M), stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellMultiple()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(100, 100M, DateTime.Now);

            stock.Sell(20, 300M, DateTime.Now);
            stock.Sell(20, 200M, DateTime.Now);
            stock.Sell(20, 400M, DateTime.Now);

            Assert.Equal(40, stock.Shares);
            Assert.Equal(900M - 60M, stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellWithLoss()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(100, 1000M, DateTime.Now);

            stock.Sell(20, 100M, DateTime.Now);

            Assert.Equal(-100M, stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellTooMuchThrowsException()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(10, 123.45M, DateTime.Now);

            Assert.Throws<ArgumentException>(() => stock.Sell(40, 345.67M, DateTime.Now));
        }

        [Fact]
        public void TestSellSendsUpdate()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(10, 123.45M, DateTime.Now);
            stock.EvtUpdate += OnStockUpdate;
            stock.Sell(4, 345.67M, DateTime.Now);

            Assert.Same(stock, updatedStock);
        }

        private void OnStockUpdate(object sender, Stock.UpdateEventArgs e)
        {
            updatedStock = e.Stock;
        }

        [Fact]
        public void TestBuyAddsTransaction()
        {
            Stock stock = new Stock("Test", "TestWKN");

            DateTime[] testDates =
            {
                new DateTime(2018, 3, 12), new DateTime(2018, 3, 13), new DateTime(2018, 3, 14)
            };

            stock.Buy(10, 100M, testDates[0]);
            stock.Buy(20, 200M, testDates[1]);
            stock.Buy(30, 300M, testDates[2]);

            Assert.Equal(3, stock.Transactions.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(enType.Buy, stock.Transactions[i].Type);
                Assert.Equal(10 * (i + 1), stock.Transactions[i].Shares);
                Assert.Equal(100 * (i + 1), stock.Transactions[i].TotalPrice);
                Assert.Equal(testDates[i], stock.Transactions[i].Date);
            }
        }

        [Fact]
        public void TestSellAddsTransaction()
        {
            Stock stock = new Stock("Test", "TestWKN");

            DateTime[] testDates =
            {
                new DateTime(2018, 3, 12), new DateTime(2018, 3, 13), new DateTime(2018, 3, 14), new DateTime(2018, 3, 15)
            };

            stock.Buy(100, 1000M, testDates[0]);

            stock.Sell(10, 100M, testDates[1]);
            stock.Sell(20, 200M, testDates[2]);
            stock.Sell(30, 300M, testDates[3]);

            Assert.Equal(4, stock.Transactions.Count);
            for (int i = 1; i < 4; i++)
            {
                Assert.Equal(enType.Sell, stock.Transactions[i].Type);
                Assert.Equal(10 * i, stock.Transactions[i].Shares);
                Assert.Equal(100 * i, stock.Transactions[i].TotalPrice);
                Assert.Equal(testDates[i], stock.Transactions[i].Date);
            }
        }

        [Fact]
        public void TestWriteStock()
        {
            Stock stock = new Stock("Test", "TestWKN");
            DateTime[] testDates =
            {
                new DateTime(2018, 3, 12), new DateTime(2018, 3, 13), new DateTime(2018, 3, 14), new DateTime(2018, 3, 15)
            };

            stock.Buy(100, 100M, testDates[0]);

            stock.Sell(20, 300M, testDates[1]);
            stock.Sell(20, 200M, testDates[2]);
            stock.Sell(20, 400M, testDates[3]);

            XDocument doc = new XDocument();
            doc.Add(stock.Write());
            doc.Save(fileName);

            XDocument imp = XDocument.Load(fileName);
            var el = imp.Element("Stock");
            var imported = Stock.FromXml(el);

            Assert.Equal(stock.Title, imported.Title);
            Assert.Equal(stock.WKN, imported.WKN);
            Assert.Equal(stock.Shares, imported.Shares);
            Assert.Equal(stock.BuyPricePerShare, imported.BuyPricePerShare);
            Assert.Equal(stock.CurrentPricePerShare, imported.CurrentPricePerShare);
            Assert.Equal(stock.RealizedRevenue, imported.RealizedRevenue);
            Assert.Equal(stock.Transactions.Count, imported.Transactions.Count);
            Assert.Equal(stock.BuyDate, imported.BuyDate);
            for (int i = 0; i < stock.Transactions.Count; i++)
            {
                var trans = stock.Transactions[i];
                var transImp = imported.Transactions[i];
                Assert.Equal(trans.Shares, transImp.Shares);
                Assert.Equal(trans.PricePerShare, transImp.PricePerShare);
                Assert.Equal(trans.TotalPrice, transImp.TotalPrice);
                Assert.Equal(trans.Type, transImp.Type);
                Assert.Equal(trans.Date, transImp.Date);
            }
        }
    }
}
