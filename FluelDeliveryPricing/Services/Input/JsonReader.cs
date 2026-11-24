using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Services.Input
{
    public class JsonReader
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
