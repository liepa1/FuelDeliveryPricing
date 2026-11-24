namespace FuelDeliveryPricing.Services.Input.Interfaces;

public interface IConsoleInput
{
    decimal ReadNonNegativeDecimal(string prompt);

    DateOnly ReadDate(string prompt);

    (DateOnly StartDate, DateOnly EndDate) ReadDateRange();
}
