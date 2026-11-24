using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models.Results
{
    public class InterestResult
    {
        public string ShipmentId { get; set; } = "";

        public DateOnly DueDate { get; set; }
        public int LateDays { get; set; }
        public decimal Interest { get; set; }
    }
}
