using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using StockView.Models;
using StockView.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace StockView.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int writeVersion = 1;
        private const string name = "StockView";

        private string fileName;
        private List<Stock> stocks;

        public ObservableCollection<StockViewModel> StockVms { get; set; }
        public StockViewModel SelectedStock
        {
            get; set;
        }

        public string TotalBuyPrice
        {
            get
            {
                return ToCurrency(stocks.Sum(x => x.BuyPricePerShare * x.Shares));
            }
        }

        public string TotalSellPrice
        {
            get
            {
                return ToCurrency(stocks.Sum(x => x.CurrentPricePerShare * x.Shares));
            }
        }

        public bool HasStocks
        {
            get
            {
                return StockVms.Count > 0;
            }
        }

        public ICommand CmdOpen { get; set; }
        public ICommand CmdAddNew { get; set; }
        public ICommand CmdBuy { get; set; }
        public ICommand CmdSell { get; set; }
        public ICommand CmdSplit { get; set; }

        public MainWindowViewModel()
        {
            stocks = new List<Stock>();
            StockVms = new ObservableCollection<StockViewModel>();
            StockVms.CollectionChanged += OnStockCollectionChanged;
            CmdOpen = new RelayCommand(CmdOpenExecute);
            CmdBuy = new RelayCommand(CmdBuyExecute);
            CmdSell = new RelayCommand(CmdSellExecute);
            CmdAddNew = new RelayCommand(CmdAddNewExecute);
            CmdSplit = new RelayCommand(CmdSplitExecute);

            fileName = Properties.Settings.Default.SaveFilePath;
            LoadFromFile(fileName);
        }

        private void OnStockCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasStocks));
        }

        private void CmdSplitExecute()
        {
            StockSplitView splitDlg = new StockSplitView(stocks, SelectedStock?.Stock);
            splitDlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            splitDlg.ShowDialog();
        }

        private void CmdAddNewExecute()
        {
            StockAddView addDlg = new StockAddView(stocks);
            addDlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            addDlg.ShowDialog();

            if (addDlg.NewStock != null)
            {
                stocks.Add(addDlg.NewStock);
                var vm = new StockViewModel(addDlg.NewStock);
                vm.EvtUpdate += OnUpdate;
                StockVms.Add(vm);

                UpdatePrices();
            }
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            UpdatePrices();
        }

        private void UpdatePrices()
        {
            RaisePropertyChanged(nameof(TotalBuyPrice));
            RaisePropertyChanged(nameof(TotalSellPrice));
        }

        private void CmdSellExecute()
        {
            StockSellView sellDlg = new StockSellView(stocks, SelectedStock?.Stock);
            sellDlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            sellDlg.ShowDialog();
        }

        private void CmdBuyExecute()
        {
            StockBuyView buyDlg = new StockBuyView(stocks, SelectedStock?.Stock);
            buyDlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            buyDlg.ShowDialog();
        }

        private void CmdOpenExecute()
        {
            if (StockVms.Count > 0)
            {
                SaveToFile();
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Lade Depotübersicht";
            dlg.Filter = "Depots (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                LoadFromFile(dlg.FileName);
            }
        }

        public void LoadFromFile(string loadFileName)
        {
            if (File.Exists(fileName))
            {
                stocks.Clear();
                StockVms.Clear();
                fileName = loadFileName;
                XDocument doc = XDocument.Load(fileName);
                var root = doc.Element(name);
                var stockCollection = root.Element("Stocks");
                foreach (var el in stockCollection.Elements())
                {
                    Stock stock = Stock.FromXml(el);
                    stocks.Add(stock);

                    var vm = new StockViewModel(stock);
                    vm.EvtUpdate += OnUpdate;
                    StockVms.Add(vm);
                }

                UpdatePrices();
            }
        }

        public void SaveToFile()
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                XDocument doc = new XDocument();
                XElement root = new XElement(name);
                root.Add(new XAttribute("Version", writeVersion));
                XElement stockCollection = new XElement("Stocks");
                stockCollection.Add(new XAttribute("Count", stocks.Count));
                foreach (var stock in stocks)
                {
                    stockCollection.Add(stock.Write());
                }
                root.Add(stockCollection);
                doc.Add(root);

                doc.Save(fileName);

                Properties.Settings.Default.SaveFilePath = fileName;
                Properties.Settings.Default.Save();
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "Speichere Depotübersicht";
                dlg.Filter = "Depots (*.xml)|*.xml";
                if (dlg.ShowDialog() == true)
                {
                    fileName = dlg.FileName;
                    SaveToFile();
                }
            }
        }

        private string ToCurrency(decimal value)
        {
            return value.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
        }
    }
}
