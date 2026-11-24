using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models.Results
{
    public class ReportResult
    {
        public int DeliveryCount { get; set; }
        public string? MostExpensiveDeliveryId { get; set; }
        public decimal TotalVat { get; set; }
    }
}
