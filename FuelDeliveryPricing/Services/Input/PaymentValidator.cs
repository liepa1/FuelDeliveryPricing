using System.Text.Json;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Models;

namespace FuelDeliveryPricing.Services.Input;

public class PaymentValidator
{
    private readonly ValidationConfiguration _configuration;

    public PaymentValidator(ValidationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Payment> Validate(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Array)
            throw new JsonException("The payments JSON must contain an array.");

        var validPayments = new List<Payment>();
        var processedIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (JsonElement element in root.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
                continue;

            if (HasMissingFields(element))
                continue;

            if (!TryParseId(element, out string id))
                continue;

            if (!TryParseDate(element, out DateOnly paidDate))
                continue;

            // Only a usable payment claims the ID, so a later valid payment
            // can be used when an earlier record for that ID was malformed.
            if (!processedIds.Add(id))
                continue;

            validPayments.Add(new Payment
            {
                Id = id,
                PaidDate = paidDate
            });
        }

        return validPayments;
    }

    private static bool HasMissingFields(JsonElement element)
    {
        return !element.TryGetProperty("id", out JsonElement id) ||
               id.ValueKind == JsonValueKind.Null ||
               !element.TryGetProperty("paidDate", out JsonElement paidDate) ||
               paidDate.ValueKind == JsonValueKind.Null;
    }

    private static bool TryParseId(JsonElement element, out string id)
    {
        if (element.TryGetProperty("id", out JsonElement idElement) &&
            idElement.ValueKind == JsonValueKind.String)
        {
            id = idElement.GetString()!;
            return true;
        }

        id = string.Empty;
        return false;
    }

    private bool TryParseDate(JsonElement element, out DateOnly date)
    {
        if (!element.TryGetProperty("paidDate", out JsonElement dateElement))
        {
            date = default;
            return false;
        }

        return DateParser.TryParseDate(dateElement, _configuration.DateFormats, out date);
    }
}
