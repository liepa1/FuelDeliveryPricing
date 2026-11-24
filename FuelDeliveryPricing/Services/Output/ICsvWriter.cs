using FuelDeliveryPricing.Models.Results;

namespace FuelDeliveryPricing.Services.Output;

public interface ICsvWriter
{
    void WritePrices(List<DeliveryResult> prices, string fileName);

    void WriteErrors(List<ShipmentError> errors);

    void WriteReport(ReportResult report, string fileName);

    void WriteInterestResults(List<InterestResult> results, string fileName);
}
