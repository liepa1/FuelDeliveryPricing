using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models.Results
{
    public class DeliveryResult
    {
        public string ShipmentId { get; set; } = "";
        public decimal NetFuelPrice { get; set; }
        public decimal LogisticsCost { get; set; }
        public decimal Excise { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public DateOnly Date { get; set; }
    }
}
