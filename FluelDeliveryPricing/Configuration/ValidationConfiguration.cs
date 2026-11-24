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
    }
}
