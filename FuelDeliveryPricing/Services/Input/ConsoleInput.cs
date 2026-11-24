using System.Globalization;
using FuelDeliveryPricing.Configuration;
using FuelDeliveryPricing.Services.Input.Interfaces;

namespace FuelDeliveryPricing.Services.Input;

public class ConsoleInput : IConsoleInput
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
            string input = ReadRequiredLine(prompt);

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
            string input = ReadRequiredLine(prompt);

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
                return (startDate, endDate);

            Console.WriteLine("End Date cannot be before Start Date. Please try again.\n");
        }
    }

    private static string ReadRequiredLine(string prompt)
    {
        if (!Console.IsInputRedirected)
            Console.Write(prompt);

        return Console.ReadLine()
            ?? throw new EndOfStreamException(
                "Input ended before all required answers were provided.");
    }
}
