using StockView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StockViewTest
{
    public class StockTest
    {
        private Stock updatedStock;

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
        }

        [Fact]
        public void TestBuy()
        {
            Stock stock = new Stock("Test", "TestWKN");

            stock.Buy(10, 123.45M);

            Assert.Equal(10, stock.Shares);
            Assert.Equal(12.345M, stock.BuyPricePerShare);
        }

        [Fact]
        public void TestBuyMultiple()
        {
            Stock stock = new Stock("Test", "TestWKN");

            stock.Buy(10, 100M);
            stock.Buy(10, 200M);
            stock.Buy(10, 300M);

            Assert.Equal(30, stock.Shares);
            Assert.Equal(20M, stock.BuyPricePerShare);
        }

        [Fact]
        public void TestBuySendsUpdate()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.EvtUpdate += OnStockUpdate;

            stock.Buy(10, 123.45M);

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
            stock.Buy(10, 123.45M);

            stock.Sell(4, 345.67M);

            Assert.Equal(6, stock.Shares);
            Assert.Equal(345.67M - (4 * 12.345M), stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellMultiple()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(100, 100M);

            stock.Sell(20, 300M);
            stock.Sell(20, 200M);
            stock.Sell(20, 400M);

            Assert.Equal(40, stock.Shares);
            Assert.Equal(900M - 60M, stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellWithLoss()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(100, 1000M);

            stock.Sell(20, 100M);

            Assert.Equal(-100M, stock.RealizedRevenue);
        }

        [Fact]
        public void TestSellTooMuchThrowsException()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(10, 123.45M);

            Assert.Throws<ArgumentException>(() => stock.Sell(40, 345.67M));
        }

        [Fact]
        public void TestSellSendsUpdate()
        {
            Stock stock = new Stock("Test", "TestWKN");
            stock.Buy(10, 123.45M);
            stock.EvtUpdate += OnStockUpdate;
            stock.Sell(4, 345.67M);

            Assert.Same(stock, updatedStock);
        }

        private void OnStockUpdate(object sender, Stock.UpdateEventArgs e)
        {
            updatedStock = e.Stock;
        }
    }
}
