using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Configuration;
using WpfApp2.Model;

namespace WpfApp2.FileHandle
{
    public class JsonFileHandle
    {
        private AppSettings _settings;
        private readonly Dictionary<string, int> SortingDic = new Dictionary<string, int>();
        public JsonFileHandle(IOptions<AppSettings> options)
        {
            this._settings = options.Value;
            for (int i = 0; i < _settings.CategoryNames.Length; i++)
            {
                SortingDic[_settings.CategoryNames[i]] = i;
            }
        }


        public IReadOnlyList<ItemDto> LoadItemsFromJson()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, _settings.ItemFileName);
            if (File.Exists(filePath) == false)
            {
                MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Array.Empty<ItemDto>();
            }
            var json = new StringBuilder();
            json.Append(File.ReadAllText(filePath));
            
            try
            {
                var items = JsonSerializer.Deserialize<List<ItemWithMeter>>(json.ToString());
                if (items==null||items.Any(item => item.LengthMm < 0) || items.Any(item => string.IsNullOrEmpty(item.Name)))
                    {
                    MessageBox.Show($"Invalid data in JSON file: LengthMm must be non-negative and Name must not be empty.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return Array.Empty<ItemDto>();
                }

                var dtos = CreateDTO(items);
                return dtos;

            }

            catch (JsonException ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Array.Empty<ItemDto>();

            }
           
        }

        private string GetCategory(double lengthMm)
        {
            string category = _settings.CategoryNames[0];
            for (int i = 0; i < _settings.CategoryRanges.Length; i++)
            {
                if (lengthMm > _settings.CategoryRanges[i])
                {
                    category = _settings.CategoryNames[i];

                }
                else
                {
                    break;
                }
            }
            return category;
        }
        /// <summary>
        /// create DTO and sort by category and length
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<ItemDto> CreateDTO(List<ItemWithMeter> items)
        {
            var dtos = items.Select(item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                LengthMm = item.LengthMm,
                Category = GetCategory(item.LengthMm)
            }).OrderByDescending(item => SortingDic.ContainsKey(item.Category) ? SortingDic[item.Category] : int.MaxValue).ThenBy(item => item.LengthMm)
            .ToList();

            return dtos;
        }
        private List<ItemDto> SortItems(IEnumerable<ItemDto> items)
        {
            return items
                .OrderByDescending(item => SortingDic.ContainsKey(item.Category) ? SortingDic[item.Category] : int.MaxValue)
                .ThenBy(item => item.LengthMm)
                .ToList();
        }
        public List<ItemDto> Reclculate(List<ItemDto> items)
        {
            items.ForEach(item =>
            {
                item.Category = GetCategory(item.LengthMm);
            });
           return SortItems(items);
        }


        public void SaveToCsv(List<ItemDto> data, string fileName ) 
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,Name,Length,LengthInch,Category");
            
                foreach (var item in data)
                {
                    builder.AppendLine(string.Join(",", item.Id, item.Name, item.LengthMm,item.LengthInch, item.Category));
                }
           
           
             File.WriteAllText(fileName, builder.ToString(), new UTF8Encoding(true));
           
        }
    }
}
