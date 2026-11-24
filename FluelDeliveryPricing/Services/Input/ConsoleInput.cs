using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelDeliveryPricing.Configuration;
using Microsoft.Extensions.Configuration;

namespace FuelDeliveryPricing.Services.Input
{
    public class ConsoleInput
    {
        private readonly ValidationConfiguration _configuration;

        public ConsoleInput(ValidationConfiguration configuration)
        {
            _configuration = configuration;
        }
        public decimal ReadNonNegativeDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string? input = Console.ReadLine();

                if (decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid input. Please enter a non-negative number.");
            }
        }

        public DateOnly ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (DateOnly.TryParseExact(
                input,
                    _configuration.DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateOnly date))
                {
                    return date;
                }

                Console.WriteLine("Invalid date. Use yyyy-MM-dd or dd/MM/yyyy.");
            }
        }

        public (DateOnly StartDate, DateOnly EndDate) ReadDateRange()
        {
            while (true)
            {
                DateOnly startDate = ReadDate("Enter Start Date: ");
                DateOnly endDate = ReadDate("Enter End Date: ");

                if (endDate >= startDate)
                {
                    return (startDate, endDate);
                }

                Console.WriteLine("End Date cannot be before Start Date. Please try again.\n");
            }
        }
    }
}
