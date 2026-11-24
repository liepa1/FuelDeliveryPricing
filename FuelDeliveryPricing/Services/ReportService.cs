using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services
{
    public class ReportService
    {
        public ReportResult GenerateReport(
        List<DeliveryResult> deliveries,
        DateOnly startDate,
        DateOnly endDate)
        {
            var deliveriesInRange = deliveries
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .ToList();

            return new ReportResult
            {
                DeliveryCount = deliveriesInRange.Count,

                MostExpensiveDeliveryId = deliveriesInRange
                    .MaxBy(x => x.GrandTotal)?.ShipmentId,

                TotalVat = deliveriesInRange.Sum(x => x.Vat)
            };

        }
    }
}
