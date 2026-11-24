using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FuelDeliveryPricing.Models;
using System.Globalization;
using FuelDeliveryPricing.Configuration;

namespace FuelDeliveryPricing.Services
{
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
            {
                throw new JsonException("The payments JSON must contain an array.");
            }

            var dateFormats = _configuration.DateFormats;
            var valid = new List<Payment>();
            var processedIds = new HashSet<string>();

            foreach (JsonElement element in root.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object ||
                    !element.TryGetProperty("id", out JsonElement elementId) ||
                    elementId.ValueKind != JsonValueKind.String ||
                    !element.TryGetProperty("paidDate", out JsonElement paidDateElement) ||
                    !TryParseDate(paidDateElement, dateFormats, out DateOnly date))
                {
                    continue;
                }

                string id = elementId.GetString()!;

                if (!processedIds.Add(id))
                {
                    continue;
                }

                valid.Add(new Payment
                {
                    Id = id,
                    PaidDate = date,
                });
            }
                return valid;


        }

        public bool TryParseDate(JsonElement element, string[] dateFormats, out DateOnly date)
        {
            string dateString = element.ValueKind == JsonValueKind.String
                ? element.GetString()!
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
    }
}
