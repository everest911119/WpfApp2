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
using WpfApp2.Services;

namespace WpfApp2.FileHandle
{
    public class JsonFileHandle
    {
        private AppSettings _settings;
        private readonly IMessageBoxService _messageBoxService;
        private readonly Dictionary<string, int> SortingDic = new Dictionary<string, int>();

        public JsonFileHandle(IOptions<AppSettings> options, IMessageBoxService messageBoxService)
        {
            _settings = options.Value;
            _messageBoxService = messageBoxService;
            //create a dictionary to store category and its index for sorting
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
                _messageBoxService.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK);
                return Array.Empty<ItemDto>();
            }
            var json = new StringBuilder();
            json.Append(File.ReadAllText(filePath));
            try
            {
                var items = JsonSerializer.Deserialize<List<ItemWithMeter>>(json.ToString());
                if (items == null || items.Any(item => item.LengthMm < 0) || items.Any(item => string.IsNullOrEmpty(item.Name)))
                {
                    int wrongId = items?.FirstOrDefault(item => item.LengthMm < 0 || string.IsNullOrEmpty(item.Name))?.Id ?? -1;
                    _messageBoxService.Show($"Invalid data in JSON file:id: {wrongId} LengthMm must be non-negative and Name must not be empty.",
                        "Error", MessageBoxButton.OK);
                    return Array.Empty<ItemDto>();
                }

                var dtos = CreateDTO(items);
                _messageBoxService.Show($"Data successfully loaded", "Success", MessageBoxButton.OK);
                return dtos;
            }
            catch (JsonException ex)
            {
                _messageBoxService.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButton.OK);
                return Array.Empty<ItemDto>();
            }
        }


        /// <summary>
        ///cate category by length and category range defined in appsettings.json 
        /// </summary>
        /// <param name="lengthMm"></param>
        /// <returns></returns>
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

        /// <summary>
        /// sort items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<ItemDto> SortItems(IEnumerable<ItemDto> items)
        {
            return items
                .OrderByDescending(item => SortingDic.ContainsKey(item.Category) ? SortingDic[item.Category] : int.MaxValue)
                .ThenBy(item => item.LengthMm)
                .ToList();
        }

        /// <summary>
        /// recalculate category and sort by category and length
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<ItemDto> Reclculate(List<ItemDto> items)
        {
            items.ForEach(item =>
            {
                item.Category = GetCategory(item.LengthMm);
            });
            return SortItems(items);
        }

        /// <summary>
        /// save to csv file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public void SaveToCsv(List<ItemDto> data, string fileName)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,Name,Length,LengthInch,Category");

            foreach (var item in data)
            {
                builder.AppendLine(string.Join(",", item.Id, item.Name, item.LengthMm, item.LengthInch, item.Category));
            }


            File.WriteAllText(fileName, builder.ToString(), new UTF8Encoding(true));

        }


        public void SaveToJson(List<ItemDto> data)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, _settings.ItemFileName);
            var itemsWithMeter = data.Select(item => new ItemWithMeter
            {
                Id = item.Id,
                Name = item.Name,
                LengthMm = item.LengthMm
            }).ToList();
            try
            {
                var json = JsonSerializer.Serialize(itemsWithMeter, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                _messageBoxService.Show($"Data successfully saved to {filePath}", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                _messageBoxService.Show($"Error saving JSON: {ex.Message}", "Error", MessageBoxButton.OK);

            }
        }
    }
}
