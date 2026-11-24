using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Services
{
    public class ShipmentReader
    {
        public JsonDocument Read(string filePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(currentDirectory, filePath);
            string json = File.ReadAllText(fullPath);

            return JsonDocument.Parse(json);
        }
    }
}
