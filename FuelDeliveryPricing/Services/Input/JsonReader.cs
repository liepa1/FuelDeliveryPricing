using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FuelDeliveryPricing.Services.Input.Interfaces;

namespace FuelDeliveryPricing.Services.Input
{
    public class JsonReader : IJsonReader
    {
        public async Task<JsonDocument> Read(string filePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(currentDirectory, filePath);
            using FileStream stream = File.OpenRead(fullPath);
            return await JsonDocument.ParseAsync(stream);
        }
    }
}
