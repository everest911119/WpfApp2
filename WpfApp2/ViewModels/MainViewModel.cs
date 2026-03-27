using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using WpfApp2.Configuration;
using WpfApp2.FileHandle;
using WpfApp2.Model;

namespace WpfApp2.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly AppSettings _settings;
        private readonly JsonFileHandle _jsonFileHandle;

        public ObservableCollection<ItemDto> Items { get; } = new();

        [ObservableProperty]
        private string headerText = string.Empty;

        public IRelayCommand LoadCommand { get; }
        public IRelayCommand ExportCommand { get; }
        public IRelayCommand RecalcCommand { get; }
        public IRelayCommand<ItemDto?> UpdateLengthCommand { get; }
        private readonly List<ItemWithMeter> cache = new List<ItemWithMeter>();
        public MainViewModel(IOptions<AppSettings> options, JsonFileHandle jsonFileHandle)
        {
            _settings = options.Value;
            _jsonFileHandle = jsonFileHandle;

            HeaderText = _settings.HeaderText;

            LoadCommand = new RelayCommand(Load);
            ExportCommand = new RelayCommand(Export);
            RecalcCommand = new RelayCommand(Recalc);
            UpdateLengthCommand = new RelayCommand<ItemDto?>(UpdateLength);
        }

        private void Load()
        {
            Items.Clear();

            var items = _jsonFileHandle.LoadItemsFromJson();
            foreach (var item in items)
            {
                Items.Add(item);
                cache.Add(new ItemWithMeter
                {
                    Id = item.Id,
                    Name = item.Name,
                    LengthMm = item.LengthMm,
                });
            }
            
        }

        private void Export()
        {
            var fileName = GetFileName(Items);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            _jsonFileHandle.SaveToCsv(Items.ToList(), fileName);
        }

        private void Recalc()
        {
            _jsonFileHandle.SaveToJson(Items.ToList());
        }

        private void UpdateLength(ItemDto? item)
        {
            if (item == null)
            {
                return;
            }
            if (item.LengthMm <= 0)
            {
                
                MessageBox.Show("Length cannot be negative.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                var nubmer = cache.Where(i=>i.Id==item.Id).FirstOrDefault().LengthMm;
                item.LengthMm = nubmer; // Reset to default value
          
            }

            var recalculated = _jsonFileHandle.Reclculate(Items.ToList());
            Items.Clear();
            cache.Clear();
            foreach (var updated in recalculated)
            {
                Items.Add(updated);
                
                cache.Add(new ItemWithMeter
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    LengthMm = updated.LengthMm,
                });
            }
        }

        private string GetFileName(IReadOnlyCollection<ItemDto> items)
        {
            if (items.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return string.Empty;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = $"InventoryMetric-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
        }
    }
}