using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Models;
using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services
{
    public class InterestService
    {
        private readonly InterestConfiguration _configuration;

        public InterestService(InterestConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<InterestResult> CalculateInterest(
    List<Payment> validPayments,
    List<DeliveryResult> task1Results)
        {
            var results = new List<InterestResult>();

            var paymentsById = validPayments.ToDictionary(
                x => x.Id,
                StringComparer.Ordinal);

            foreach (var delivery in task1Results)
            {
                DateOnly paymentDate = paymentsById.TryGetValue(
                    delivery.ShipmentId, out var payment)
                        ? payment.PaidDate
                        : _configuration.ReportDate;

                DateOnly dueDate = delivery.Date.AddDays(
                    _configuration.PaymentDueDays);

                int lateDays = Math.Max(
                    0,
                    paymentDate.DayNumber - dueDate.DayNumber);

                decimal interest = CalculateAmount(
                    delivery.GrandTotal,
                    dueDate,
                    paymentDate);

                results.Add(new InterestResult
                {
                    ShipmentId = delivery.ShipmentId,
                    DueDate = dueDate,
                    LateDays = lateDays,
                    Interest = interest
                });
            }

            return results;
        }

        private decimal CalculateAmount(
            decimal grandTotal,
            DateOnly dueDate,
            DateOnly paymentDate)
        {
            DateOnly firstLateDay = dueDate.AddDays(1);

            if (paymentDate < firstLateDay)
                return 0m;

            DateOnly rateChangeDate = _configuration.RateChangeDate;

            int daysBeforeChange = 0;
            int daysAfterChange = 0;

            if (firstLateDay < rateChangeDate)
            {
                DateOnly lastDayBeforeChange =
                    paymentDate < rateChangeDate
                        ? paymentDate
                        : rateChangeDate.AddDays(-1);

                daysBeforeChange =
                    lastDayBeforeChange.DayNumber - firstLateDay.DayNumber + 1;
            }

            if (paymentDate >= rateChangeDate)
            {
                DateOnly firstDayAfterChange =
                    firstLateDay > rateChangeDate
                        ? firstLateDay
                        : rateChangeDate;

                daysAfterChange =
                    paymentDate.DayNumber - firstDayAfterChange.DayNumber + 1;
            }

            decimal interest =
                grandTotal *
                (_configuration.RateBeforeChange * daysBeforeChange +
                 _configuration.RateAfterChange * daysAfterChange) / 365m;

            return Math.Round(
                interest,
                2,
                MidpointRounding.AwayFromZero);
        }
    }
 }
