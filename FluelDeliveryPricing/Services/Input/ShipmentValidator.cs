using System.Text.Json;
using System.Globalization;
using FuelDeliveryPricing.Models.Enums;
using FuelDeliveryPricing.Models;
using Microsoft.VisualBasic;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services.Input;
public class ShipmentValidator
{
    private readonly ValidationConfiguration _configuration;
    public ShipmentValidator(ValidationConfiguration configuration)
    {
        _configuration = configuration;
    }
    public (List<Shipment> Valid, List<ShipmentError> Errors) Validate(JsonElement root)
    {

        var valid = new List<Shipment>();
        var errors = new List<ShipmentError>();

        var processedIds = new HashSet<string>();


        foreach (JsonElement element in root.EnumerateArray())
        {
            if (root.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException("The shipments JSON must contain an array.");
            }
            JsonElement elementId = default;

            bool hasValidId =
                element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty("id", out elementId) &&
                elementId.ValueKind == JsonValueKind.String;

            string id = hasValidId ? elementId.GetString()! : "";

            if (hasValidId && !processedIds.Add(id))
            {
                AddError(errors, id, ErrorCode.DUPLICATE_ID);
                continue;
            }

            // 2. Check required fields
            if (HasMissingFields(element))
            {
                AddError(errors, id, ErrorCode.MISSING_FIELD);
                continue;
            }

            // 3. Validate fuel type
            if (!TryParseFuelType(element, out FuelType? fuelType))
            {
                AddError(errors, id, ErrorCode.UNKNOWN_TYPE);
                continue;

            }

            // 4. Validate client
            if (!TryParseClientType(element, out ClientType? clientType))
            {
                AddError(errors, id, ErrorCode.UNKNOWN_CLIENT);
                continue;
            }

            // 5. Validate date
            if (!TryParseDate(element, out DateOnly date))
            {
                AddError(errors, id, ErrorCode.INVALID_DATE);
                continue;
            }

            // 6. Validate numeric values
            if (!TryParseNumerics(element, out decimal volume, out decimal distance, out decimal basePrice))
            {
                AddError(errors, id, ErrorCode.INVALID_VALUE);
                continue;
            }

            // 7. All validations passed - create Shipment
            valid.Add(new Shipment
            {
                Id = id,
                Type = fuelType.Value,
                Client = clientType.Value,
                Date = date,
                Volume = volume,
                Distance = distance,
                BasePrice = basePrice
            });
        }

        return (valid, errors);
    }

    private bool HasMissingFields(JsonElement element)
    {

        // 1. Check required fields
        string[] requiredFields = _configuration.RequiredFields;


        foreach (string field in requiredFields)
        {
            if (!element.TryGetProperty(field, out JsonElement value)
                || value.ValueKind == JsonValueKind.Null)
            {
                return true;
            }
        }
        return false;

    }

    private bool TryParseFuelType(JsonElement element, out FuelType? fuelType)
    {
        string type = element.GetProperty("type")
                .ValueKind == JsonValueKind.String
                ? element.GetProperty("type").GetString()!.Trim().ToUpperInvariant()
                : "";

        fuelType = type switch
        {
            "GASOLINE" => FuelType.Gasoline,
            "DIESEL" => FuelType.Diesel,
            "LPG" => FuelType.LPG,
            _ => null
        };

        if (fuelType == null)
        {
            return false;
        }

        return true;

    }

    private bool TryParseClientType(JsonElement element, out ClientType? clientType)
    {

        string client = element.GetProperty("client")
                .ValueKind == JsonValueKind.String
                ? element.GetProperty("client").GetString()!.Trim().ToUpperInvariant()
                : "";

        clientType = client switch
        {
            "PRIVATE" => ClientType.Private,
            "EXEMPT" => ClientType.Exempt,
            _ => null
        };
        if (clientType == null)
        {
            return false;
        }
        return true;
    }

    private bool TryParseDate(JsonElement element, out DateOnly date)
    {
        string[] dateFormats = _configuration.DateFormats;
        string dateString = element.GetProperty("date")
                .ValueKind == JsonValueKind.String
                ? element.GetProperty("date").GetString()!
                : "";

        if (!DateOnly.TryParseExact(
            dateString,
            dateFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date))
        {
            return false;

        }
        return true;
    }

    private bool TryParseNumerics(JsonElement element, out decimal volume, out decimal distance, out decimal basePrice)
    {
        volume = 0;
        distance = 0;
        basePrice = 0;
        if (element.GetProperty("volume").ValueKind != JsonValueKind.Number
               || !element.GetProperty("volume").TryGetDecimal(out volume)
               || element.GetProperty("distance").ValueKind != JsonValueKind.Number
               || !element.GetProperty("distance").TryGetDecimal(out distance)
               || element.GetProperty("basePrice").ValueKind != JsonValueKind.Number
               || !element.GetProperty("basePrice").TryGetDecimal(out basePrice)
               || volume <= 0
               || distance < 0
               || basePrice < 0)
        { return false; }
        return true;
    }

    private void AddError(List<ShipmentError> errors, string id, ErrorCode code)
    {
        errors.Add(new ShipmentError
        {
            ShipmentId = id,
            Code = code
        });
    }



}
