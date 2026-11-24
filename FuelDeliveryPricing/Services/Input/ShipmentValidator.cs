using System.Text.Json;
using FuelDeliveryPricing.Models.Enums;
using FuelDeliveryPricing.Models;
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

        if (root.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("The shipments JSON must contain an array.");
        }

        foreach (JsonElement element in root.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                AddError(errors, "", ErrorCode.MISSING_FIELD);
                continue;
            }

            JsonElement elementId = default;

            bool hasValidId =
                element.TryGetProperty("id", out elementId) &&
                elementId.ValueKind == JsonValueKind.String;

            string id = hasValidId ? elementId.GetString()! : "";

            if (hasValidId && !processedIds.Add(id))
            {
                AddError(errors, id, ErrorCode.DUPLICATE_ID);
                continue;
            }

            if (HasMissingFields(element))
            {
                AddError(errors, id, ErrorCode.MISSING_FIELD);
                continue;
            }

            if (!TryParseFuelType(element, out FuelType? fuelType))
            {
                AddError(errors, id, ErrorCode.UNKNOWN_TYPE);
                continue;

            }

            if (!TryParseClientType(element, out ClientType? clientType))
            {
                AddError(errors, id, ErrorCode.UNKNOWN_CLIENT);
                continue;
            }

            if (!TryParseDate(element, out DateOnly date))
            {
                AddError(errors, id, ErrorCode.INVALID_DATE);
                continue;
            }

            if (!TryParseNumerics(element, out decimal volume, out decimal distance, out decimal basePrice))
            {
                AddError(errors, id, ErrorCode.INVALID_VALUE);
                continue;
            }

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
        if (!element.TryGetProperty("date", out JsonElement dateElement))
        {
            date = default;
            return false;
        }

        return DateParser.TryParseDate(dateElement, _configuration.DateFormats, out date);
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
