using StockView.Models;
using StockView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewTest
{
    public class StockViewModelTest
    {
        private Stock stock;
        private StockViewModel vm;

        public StockViewModelTest()
        {
            stock = new Stock("Test", "TestWKN");
        }
    }
}
