using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Configuration
{
    public class ValidationConfiguration
    {
        public string[] DateFormats { get; set; } = [];
        public string[] RequiredFields { get; set; } = [];

        public void Validate()
        {
            if (DateFormats is null || DateFormats.Length == 0 ||
                DateFormats.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException("At least one non-empty date format must be configured.");

            if (RequiredFields is null || RequiredFields.Length == 0 ||
                RequiredFields.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException("RequiredFields must contain non-empty field names.");

            
        }
    }
}
