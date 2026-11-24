using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Models.Enums;

namespace FuelDeliveryPricing.Models.Results
{
    public class ShipmentError
    {
        public string ShipmentId { get; set; }
        public ErrorCode Code { get; set; }
    }
}
