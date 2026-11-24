using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Models
{
    public class Payment
    {
        public string Id { get; set; }
        public DateOnly PaidDate { get; set; }
    }
}
