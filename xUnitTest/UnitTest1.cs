using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using WpfApp2;
using WpfApp2.Configuration;
using WpfApp2.FileHandle;
using WpfApp2.Model;
using Xunit;

namespace xUnitTest
    {
       

        public class JsonFileHandleTests
        {
            [Fact]
            public void LoadItemsFromJson_ValidFile_ReturnsSortedDtos()
            {
                var settings = new AppSettings
                {
                    ItemFileName = "Item-test1.json",
                    CategoryRanges = new[] { 0, 500, 1000, 1500 },
                    CategoryNames = new[] { "Small", "Medium", "Large", "Oversize" }
                };

                var filePath = Path.Combine(AppContext.BaseDirectory, settings.ItemFileName);
                var items = new List<ItemWithMeter>
            {
                new() { Id = 1, Name = "A", LengthMm = 1200 }, // Large
                new() { Id = 2, Name = "B", LengthMm = 200 },  // Small
                new() { Id = 3, Name = "C", LengthMm = 1600 }, // Oversize
                new() { Id = 4, Name = "D", LengthMm = 800 }   // Medium
            };

                File.WriteAllText(filePath, JsonSerializer.Serialize(items));

                try
                {
                    var handle = new JsonFileHandle(Options.Create(settings));

                    var res = handle.LoadItemsFromJson();

                    Assert.Equal(4, result.Count);
                    Assert.Equal(new[] { 3, 1, 4, 2 }, res.Select(x => x.Id).ToArray());
                    Assert.Equal(new[] { "Oversize", "Large", "Medium", "Small" }, res.Select(x => x.Category).ToArray());
                }
                finally
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            [Fact]
            public void RecalTest()
            {
                var settings = new AppSettings
                {
                    ItemFileName = "Item-1.json",
                    CategoryRanges = new[] { 0, 500, 1000, 1500 },
                    CategoryNames = new[] { "Small", "Medium", "Large", "Oversize" }
                };

                var handle = new JsonFileHandle(Options.Create(settings));

                var items = new List<ItemDto>
            {
                new() { Id = 1, Name = "Prod 1", LengthMm = 1200, Category = "Small" },
                new() { Id = 2, Name = "Prod 2", LengthMm = 200, Category = "Oversize" },
                new() { Id = 3, Name = "Prod 3", LengthMm = 1600, Category = "Medium" }
            };

                var result = handle.Reclculate(items);

                Assert.Equal(new[] { 3, 1, 2 }, result.Select(x => x.Id).ToArray());
                Assert.Equal(new[] { "Oversize", "Large", "Small" }, result.Select(x => x.Category).ToArray());
            }

            [Fact]
            public void SaveToCsv_WritesCsv()
            {
                var settings = new AppSettings
                {
                    ItemFileName = "Item-unused.json",
                    CategoryRanges = new[] { 0, 500, 1000, 1500 },
                    CategoryNames = new[] { "Small", "Medium", "Large", "Oversize" }
                };

                var handle = new JsonFileHandle(Options.Create(settings));
                var filePath = Path.Combine(Path.GetTempPath(), $"items-{Guid.NewGuid():N}.csv");

                try
                {
                    var data = new List<ItemDto>
                {
                    new() { Id = 1, Name = "Prod 1", LengthMm = 10, Category = "Small" }
                };

                    handle.SaveToCsv(data, filePath);

                    var lines = File.ReadAllLines(filePath);

                    Assert.Equal("Id,Name,Length,LengthInch,Category", lines[0]);
                   
                    Assert.Contains("3/8\"", lines[1]);
                    Assert.EndsWith(",Small", lines[1]);
                }
                finally
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
    }