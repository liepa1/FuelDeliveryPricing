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

        public void Validate()
        {
            if (PaymentDueDays < 0)
                throw new InvalidOperationException("PaymentDueDays must be non-negative.");

            if (ReportDate == default || RateChangeDate == default)
                throw new InvalidOperationException("ReportDate and RateChangeDate must be configured.");

            if (RateBeforeChange < 0 || RateBeforeChange > 1 ||
                RateAfterChange < 0 || RateAfterChange > 1)
            {
                throw new InvalidOperationException("Interest rates must be between 0 and 1.");
            }
        }
    }
}
