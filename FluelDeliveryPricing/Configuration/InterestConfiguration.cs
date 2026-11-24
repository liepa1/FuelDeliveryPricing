using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelDeliveryPricing.Configuration
{
    public class InterestConfiguration
    {
        public int PaymentDueDays { get; set; }
        public DateOnly ReportDate { get; set; }
        public DateOnly RateChangeDate { get; set; }

        public decimal RateBeforeChange { get; set; }
        public decimal RateAfterChange { get; set; }
    }
}
