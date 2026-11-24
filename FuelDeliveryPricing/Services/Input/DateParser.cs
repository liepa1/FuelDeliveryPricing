using System;
using System.Globalization;
using System.Text.Json;

namespace FuelDeliveryPricing.Services.Input
{
    public static class DateParser
    {
        public static bool TryParseDate(JsonElement element, string[] dateFormats, out DateOnly date)
        {
            string dateString = element.ValueKind == JsonValueKind.String
                ? element.GetString()!
                : "";

            return DateOnly.TryParseExact(
                dateString,
                dateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
        }
    }
}
