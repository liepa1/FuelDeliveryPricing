using System.Text.Json;

namespace FuelDeliveryPricing.Services.Input.Interfaces;

public interface IJsonReader
{
    Task<JsonDocument> Read(string filePath);
}
