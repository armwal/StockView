using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using StockView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockView
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SaveToFile();
                MessageBox.Show("Änderungen gespeichert!", "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnAutoGeneratingColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "Aktueller Kurs")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void OnFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Vor dem Schließen Änderungen abspeichern?", "Speichern?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.SaveToFile();
                }
            }
        }
    }
}
