//using Microsoft.Extensions.Options;
//using Microsoft.Win32;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Text;
//using System.Text.Json;
//using System.Windows;
//using WpfApp2.Configuration;
//using WpfApp2.FileHandle;
//using WpfApp2.Model;
//using WpfApp2.ViewModels;

//namespace WpfApp2
//{
//    /// <summary>
//    /// Interaction logic for MainWindow.xaml
//    /// </summary>
//    public partial class MainWindow : Window
//    {
//        private readonly AppSettings _settings;
//        private readonly ObservableCollection<ItemDto> _items = new();

//        private readonly JsonFileHandle jsonFileHandle;
//        public MainWindow(IOptions<AppSettings> options,
//JsonFileHandle jsonFileHandle)
//        {
//            InitializeComponent();

//            _settings = options.Value;
//            HeaderTextBlock.Text = _settings.HeaderText;

//            DATA_GRID.ItemsSource = _items;
//            DATA_GRID_SECOND.ItemsSource = _items;
//            this.jsonFileHandle = jsonFileHandle;
//        }

//        public MainWindow(MainViewModel viewModel)
//        {
//            InitializeComponent();
//            DataContext = viewModel;
//        }

//        private void LoadButton_Click(object sender, RoutedEventArgs e)
//        {
//            _items.Clear();

//            var items = jsonFileHandle.LoadItemsFromJson();

//            foreach (var item in items)
//            {
//                _items.Add(item);
//            }

//        }

//        private void ExportMeter_Click(object sender, RoutedEventArgs e)
//        {
//            var fileName = GetFileName(_items.ToList());
//            jsonFileHandle.SaveToCsv(_items.ToList(), fileName);
//        }

//        private void ReCalc_Click(object sender, RoutedEventArgs e)
//        {
//           var item  = jsonFileHandle.Reclculate(_items.ToList());
//            _items.Clear();
//            foreach (var i in item)
//            {
//                _items.Add(i);
//            }

//        }

//        private string GetFileName(List<ItemDto> items)
//        {
//            if (items.Count == 0)
//            {
//                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
//                return "";
//            }
//            var dialog = new SaveFileDialog
//            {
//                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
//                FileName = $"InventoryMetric-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv"
//            };
//            if (dialog.ShowDialog(this) != true)
//            {
//                return "";
//            }
//            return dialog.FileName;
//        }
//    }
//}
using System.Windows;
using WpfApp2.ViewModels;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}